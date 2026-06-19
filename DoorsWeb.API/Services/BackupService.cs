using System.Diagnostics;
using System.Text;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.DTO;
using Microsoft.AspNetCore.SignalR;
using Npgsql;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Database backup/restore via PostgreSQL's <c>pg_dump</c> / <c>pg_restore</c> command-line tools
    /// (installed in the API container image). Backups are written in pg_dump's custom format
    /// (<c>.dump</c>) — compressed and restorable with pg_restore. Unlike the old SQL Server SMO path,
    /// the API process itself runs the tools over TCP, so the backup directory only needs to be
    /// writable by the API (no shared database service-account path).
    ///
    /// Live percent-complete is pushed to the initiating client over <see cref="BackupHub"/>:
    ///  - <b>Backup</b> (<c>BackupProgress</c>): parsed from <c>pg_dump --verbose</c>, one step per table
    ///    as its data is dumped. Real but coarse — the bar pauses on a single dominant table.
    ///  - <b>Restore</b> (<c>RestoreProgress</c>): the dump is streamed into <c>pg_restore</c> via stdin
    ///    and progress is the bytes fed divided by the file size — an exact, smooth percentage.
    /// Both are capped below 100% until the tool actually exits, and all values are clamped to [0,100].
    /// </summary>
    public class BackupService : IBackupService
    {
        private readonly string _backupDirectory;
        private readonly NpgsqlConnectionStringBuilder _conn;
        private readonly IHubContext<BackupHub> _hub;

        public BackupService(IConfiguration configuration, IHubContext<BackupHub> hub)
        {
            _hub = hub;

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            _conn = new NpgsqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(_conn.Database))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' has no Database.");
            }

            var configured = configuration["Backup:Directory"];
            _backupDirectory = string.IsNullOrWhiteSpace(configured)
                ? Path.Combine(AppContext.BaseDirectory, "Backups")
                : configured;

            Directory.CreateDirectory(_backupDirectory);
        }

        public List<BackupFileDto> GetBackups()
        {
            if (!Directory.Exists(_backupDirectory))
            {
                return new List<BackupFileDto>();
            }

            return Directory.EnumerateFiles(_backupDirectory, "*.dump")
                .Select(path => new FileInfo(path))
                .OrderByDescending(f => f.CreationTimeUtc)
                .Select(f => new BackupFileDto
                {
                    FileName = f.Name,
                    SizeBytes = f.Length,
                    CreatedUtc = f.CreationTimeUtc
                })
                .ToList();
        }

        public async Task<BackupOperationResult> CreateBackup(CreateBackupRequest request, CancellationToken cancellationToken = default)
        {
            var fileName = string.IsNullOrWhiteSpace(request.FileName)
                ? $"{_conn.Database}_{DateTime.Now:yyyyMMdd_HHmmss}.dump"
                : EnsureDumpExtension(Path.GetFileName(request.FileName));

            var fullPath = Path.Combine(_backupDirectory, fileName);
            var connectionId = request.ConnectionId;

            // Denominator for table-by-table progress. If we can't count (0), the bar just stays at 0%.
            int total = await CountDataTables(cancellationToken);
            int done = 0;
            int lastPct = -1;

            // pg_dump --verbose logs "dumping contents of table ..." as it STARTS each table's data.
            // Seeing that line for table N means tables 1..N-1 have finished, so credit (done - 1) —
            // this never over-reports (the biggest table could be last).
            void OnStderrLine(string line)
            {
                if (total <= 0) return;
                if (!line.Contains("dumping contents of table", StringComparison.Ordinal)) return;

                done++;
                int pct = (int)Math.Clamp((long)(done - 1) * 100 / total, 0L, 99L);
                if (pct != lastPct)
                {
                    lastPct = pct;
                    Report(connectionId, "BackupProgress", pct);
                }
            }

            // Custom format (-Fc) is compressed and restorable with pg_restore (selective, --clean, etc).
            var args = new List<string>
            {
                "--format=custom",
                $"--compress={(request.Compress ? "6" : "0")}",
                "--no-owner",
                "--no-privileges",
                "--verbose", // emits per-table "dumping contents of table" lines we parse for progress
                "--file", fullPath,
                "--dbname", _conn.Database!,
            };

            var (ok, error) = await RunTool("pg_dump", args, onStderrLine: OnStderrLine, ct: cancellationToken);
            if (!ok)
            {
                try { if (File.Exists(fullPath)) File.Delete(fullPath); } catch { /* best-effort */ }
                return new BackupOperationResult
                {
                    Success = false,
                    Message = $"Backup failed: {error}",
                    FileName = fileName
                };
            }

            Report(connectionId, "BackupProgress", 100);
            return new BackupOperationResult
            {
                Success = true,
                Message = $"Backup completed: {fileName}",
                FileName = fileName
            };
        }

        public async Task<BackupOperationResult> RestoreBackup(RestoreBackupRequest request, CancellationToken cancellationToken = default)
        {
            var fileName = EnsureDumpExtension(Path.GetFileName(request.FileName ?? ""));
            var fullPath = Path.Combine(_backupDirectory, fileName);

            if (!File.Exists(fullPath))
            {
                return new BackupOperationResult
                {
                    Success = false,
                    Message = $"Backup file '{fileName}' was not found.",
                    FileName = fileName
                };
            }

            var connectionId = request.ConnectionId;
            long total = Math.Max(1, new FileInfo(fullPath).Length);

            // --clean --if-exists drops existing objects before recreating them, so the dump replaces
            // the current contents. Idle pooled API connections don't hold table locks, so the drops
            // proceed; an in-flight query against the target tables could block the restore.
            //
            // The dump is fed via stdin (no file argument) so we can measure progress as bytes-streamed
            // / file-size — pg_restore reads a custom-format archive from stdin sequentially.
            var args = new List<string>
            {
                "--clean",
                "--if-exists",
                "--no-owner",
                "--no-privileges",
                "--dbname", _conn.Database!,
            };

            // Stream the dump into pg_restore's stdin, reporting bytes fed / total. Capped at 99% so the
            // bar doesn't hit 100% until the tool finishes its post-data (index/constraint) phase.
            async Task FeedStdin(Stream stdin, CancellationToken ct)
            {
                await using var file = File.OpenRead(fullPath);
                var buffer = new byte[81920];
                long fed = 0;
                int lastPct = -1;
                int read;
                while ((read = await file.ReadAsync(buffer, ct)) > 0)
                {
                    await stdin.WriteAsync(buffer.AsMemory(0, read), ct);
                    fed += read;
                    int pct = (int)Math.Clamp(fed * 100 / total, 0L, 99L);
                    if (pct != lastPct)
                    {
                        lastPct = pct;
                        Report(connectionId, "RestoreProgress", pct);
                    }
                }
                await stdin.FlushAsync(ct);
            }

            var (ok, error) = await RunTool("pg_restore", args, feedStdin: FeedStdin, ct: cancellationToken);
            if (!ok)
            {
                return new BackupOperationResult
                {
                    Success = false,
                    Message = $"Restore failed: {error}",
                    FileName = fileName
                };
            }

            Report(connectionId, "RestoreProgress", 100);
            return new BackupOperationResult
            {
                Success = true,
                Message = $"Restore completed from: {fileName}",
                FileName = fileName
            };
        }

        // Runs a libpq client tool against the configured server, passing the password via PGPASSWORD
        // and host/port/user via flags. Optionally observes each stderr line (for verbose progress) and
        // optionally feeds the process's stdin. Returns (exit code == 0, captured stderr on failure).
        private async Task<(bool ok, string error)> RunTool(
            string tool,
            IReadOnlyList<string> extraArgs,
            Action<string>? onStderrLine = null,
            Func<Stream, CancellationToken, Task>? feedStdin = null,
            CancellationToken ct = default)
        {
            var psi = new ProcessStartInfo
            {
                FileName = tool,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = feedStdin is not null,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            psi.ArgumentList.Add("--host");
            psi.ArgumentList.Add(string.IsNullOrWhiteSpace(_conn.Host) ? "localhost" : _conn.Host!);
            psi.ArgumentList.Add("--port");
            psi.ArgumentList.Add((_conn.Port == 0 ? 5432 : _conn.Port).ToString());
            psi.ArgumentList.Add("--username");
            psi.ArgumentList.Add(_conn.Username ?? "postgres");
            psi.ArgumentList.Add("--no-password"); // never prompt; use PGPASSWORD
            foreach (var a in extraArgs) psi.ArgumentList.Add(a);

            psi.Environment["PGPASSWORD"] = _conn.Password ?? "";

            using var process = new Process { StartInfo = psi };
            var stderr = new StringBuilder();
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is null) return;
                stderr.AppendLine(e.Data);
                onStderrLine?.Invoke(e.Data);
            };
            process.OutputDataReceived += (_, _) => { };

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                return (false, $"Could not start {tool} (is the PostgreSQL client installed?): {ex.Message}");
            }

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            if (feedStdin is not null)
            {
                try
                {
                    await using var stdin = process.StandardInput.BaseStream;
                    await feedStdin(stdin, ct);
                }
                catch (IOException)
                {
                    // pg_restore closed the pipe early (typically because it hit a restore error). The
                    // real cause is in stderr and surfaces via the non-zero exit code below.
                }
                catch (OperationCanceledException)
                {
                    try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch { /* best-effort */ }
                    throw;
                }
            }

            try
            {
                await process.WaitForExitAsync(ct);
            }
            catch (OperationCanceledException)
            {
                try { if (!process.HasExited) process.Kill(entireProcessTree: true); } catch { /* best-effort */ }
                throw;
            }

            if (process.ExitCode == 0) return (true, "");

            var message = stderr.ToString().Trim();
            return (false, message.Length > 0 ? message : $"{tool} exited with code {process.ExitCode}.");
        }

        // Counts base tables in the public schema — the denominator for pg_dump table-by-table progress.
        // Returns 0 if it can't connect, in which case backup progress simply stays at 0%.
        private async Task<int> CountDataTables(CancellationToken ct)
        {
            try
            {
                await using var conn = new NpgsqlConnection(_conn.ConnectionString);
                await conn.OpenAsync(ct);
                await using var cmd = new NpgsqlCommand(
                    "SELECT count(*) FROM information_schema.tables " +
                    "WHERE table_schema = 'public' AND table_type = 'BASE TABLE';", conn);
                var result = await cmd.ExecuteScalarAsync(ct);
                return result is null or DBNull ? 0 : Convert.ToInt32(result);
            }
            catch
            {
                return 0;
            }
        }

        // Fire-and-forget percent push to the initiating client. Clamped to [0,100] so a miscount or an
        // error path can never drive the client's progress bar past 100% (or below 0%).
        private void Report(string? connectionId, string method, int percent)
        {
            if (string.IsNullOrEmpty(connectionId)) return;
            _ = _hub.Clients.Client(connectionId).SendAsync(method, Math.Clamp(percent, 0, 100));
        }

        private static string EnsureDumpExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.dump";
            }
            return fileName.EndsWith(".dump", StringComparison.OrdinalIgnoreCase) ? fileName : fileName + ".dump";
        }
    }
}
