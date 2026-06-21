using System.Diagnostics;
using DoorsWeb.API.Services.Interfaces;
using DoorsWeb.Shared.Auth;

namespace DoorsWeb.API.Services
{
    /// <summary>
    /// Enforces the password policy. The breached/common-password screen reads a bundled offline list
    /// (SecLists xato-net top 1,000,000) once at startup into a case-insensitive <see cref="HashSet{T}"/>,
    /// so each check is an O(1) lookup with no network call — the system stays fully offline.
    ///
    /// If the list file is missing the service degrades gracefully: a warning is logged and nothing is
    /// treated as breached; the <see cref="PasswordPolicy.MinLength"/> length rule still applies.
    /// Registered as a singleton so the ~1M-entry set is loaded just once.
    /// </summary>
    public class PasswordPolicyService : IPasswordPolicyService
    {
        private readonly HashSet<string> _breached;

        public PasswordPolicyService(IWebHostEnvironment env, ILogger<PasswordPolicyService> logger)
        {
            // OrdinalIgnoreCase so "Password123456" is caught by the list's "password123456" entry.
            _breached = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var path = ResolvePath(env);
            if (path is null)
            {
                logger.LogWarning(
                    "Breached-password list not found (looked for Resources/common-passwords.txt). "
                    + "Breach screening is disabled; the {MinLength}-character length rule still applies.",
                    PasswordPolicy.MinLength);
                return;
            }

            var sw = Stopwatch.StartNew();
            foreach (var line in File.ReadLines(path))
            {
                var pw = line.Trim();
                if (pw.Length > 0) _breached.Add(pw);
            }
            sw.Stop();
            logger.LogInformation(
                "Loaded {Count:N0} breached/common passwords from {Path} in {Ms} ms.",
                _breached.Count, path, sw.ElapsedMilliseconds);
        }

        public bool IsBreached(string password) =>
            !string.IsNullOrEmpty(password) && _breached.Contains(password);

        public PasswordValidationResult Validate(string? password)
        {
            var length = PasswordPolicy.ValidateLength(password);
            if (!length.IsValid) return length;

            return IsBreached(password!)
                ? PasswordValidationResult.Fail(PasswordPolicy.BreachedMessage)
                : PasswordValidationResult.Ok();
        }

        // The list is copied next to the build output (Content + PreserveNewest). ContentRootPath is
        // the usual home; fall back to the assembly directory to cover odd hosting layouts.
        private static string? ResolvePath(IWebHostEnvironment env)
        {
            foreach (var root in new[] { env.ContentRootPath, AppContext.BaseDirectory })
            {
                if (string.IsNullOrEmpty(root)) continue;
                var candidate = Path.Combine(root, "Resources", "common-passwords.txt");
                if (File.Exists(candidate)) return candidate;
            }
            return null;
        }
    }
}
