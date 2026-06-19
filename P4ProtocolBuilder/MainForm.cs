using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace P4ProtocolBuilder;

public sealed class MainForm : Form
{
    private readonly Font _mono = new("Consolas", 9F);

    // Connection
    private readonly TextBox _txtIp = new() { Text = "192.168.1.100", Width = 130 };
    private readonly NumericUpDown _numPort = new() { Minimum = 0, Maximum = 65535, Value = 3000, Width = 70 };
    private readonly NumericUpDown _numLocalPort = new() { Minimum = 0, Maximum = 65535, Value = 0, Width = 70 };
    private readonly Button _btnOpen = new() { Text = "Open / Listen", Width = 110, AutoSize = true };
    private readonly Label _lblStatus = new() { Text = "Closed.", AutoSize = true, ForeColor = Color.DimGray };

    // Packet builder
    private readonly ComboBox _cboPreset = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 340 };
    private readonly TextBox _txtDest = new() { Text = "51126", Width = 120 };
    private readonly TextBox _txtSrc = new() { Text = "1", Width = 120 };
    private readonly TextBox _txtBsn = new() { Text = "01", Width = 50 };
    private readonly TextBox _txtCmd1 = new() { Text = "0B", Width = 50 };
    private readonly TextBox _txtCmd2 = new() { Text = "01", Width = 50 };
    private readonly TextBox _txtData = new() { Width = 540 };
    private readonly Label _lblDataLen = new() { Text = "0", AutoSize = true, Font = new Font("Consolas", 9F) };
    private readonly Label _lblHint = new() { AutoSize = true, ForeColor = Color.DimGray, MaximumSize = new Size(820, 0) };
    private readonly TextBox _txtPreview = new()
    {
        ReadOnly = true,
        Multiline = true,
        WordWrap = true,
        Width = 600,
        Height = 66,
        ScrollBars = ScrollBars.Vertical,
        Font = new Font("Consolas", 9F),
        BackColor = Color.White,
    };
    private readonly Label _lblInfo = new() { AutoSize = true, ForeColor = Color.DimGray, Font = new Font("Consolas", 9F) };
    private readonly Button _btnSend = new() { Text = "Send", Width = 90, Height = 40, AutoSize = false };

    // Log
    private readonly RichTextBox _rtbLog = new() { ReadOnly = true, Dock = DockStyle.Fill, BackColor = Color.White };
    private readonly CheckBox _chkAutoScroll = new() { Text = "Auto-scroll", Checked = true, AutoSize = true };
    private readonly Button _btnClear = new() { Text = "Clear log", AutoSize = true };

    // Networking state
    private UdpClient? _udp;
    private CancellationTokenSource? _cts;

    private byte[]? _currentPacket;
    private bool _suppressPreview;

    public MainForm()
    {
        Text = "P4 UDP Protocol Builder — Progeny Doc 4002 v4.68";
        ClientSize = new Size(900, 690);
        MinimumSize = new Size(720, 560);
        Font = SystemFonts.MessageBoxFont ?? new Font("Segoe UI", 9F);
        _rtbLog.Font = _mono;

        BuildUi();
        WireEvents();

        foreach (var preset in CommandPreset.All)
            _cboPreset.Items.Add(preset);
        _cboPreset.SelectedIndex = 0; // Custom / Raw
        _lblHint.Text = CommandPreset.All[0].Hint;

        UpdatePreview();
    }

    private void BuildUi()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(6) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 285));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        root.Controls.Add(BuildConnectionGroup(), 0, 0);
        root.Controls.Add(BuildPacketGroup(), 0, 1);
        root.Controls.Add(BuildLogGroup(), 0, 2);
        Controls.Add(root);
    }

    private GroupBox BuildConnectionGroup()
    {
        var stack = NewStack();
        stack.Controls.Add(Row(
            Lbl("Target IP"), _txtIp,
            Lbl("Port"), _numPort,
            Lbl("Local port (0 = auto)"), _numLocalPort,
            _btnOpen));
        stack.Controls.Add(Row(_lblStatus));
        return Group("Connection", stack);
    }

    private GroupBox BuildPacketGroup()
    {
        var stack = NewStack();
        stack.Controls.Add(Row(Lbl("Command"), _cboPreset));
        stack.Controls.Add(Row(
            Lbl("Destination (dec or 0x..)"), _txtDest,
            Lbl("Source"), _txtSrc,
            Lbl("BSN (hex)"), _txtBsn));
        stack.Controls.Add(Row(
            Lbl("Command 1 (hex)"), _txtCmd1,
            Lbl("Command 2 (hex)"), _txtCmd2,
            Lbl("Data length"), _lblDataLen));
        stack.Controls.Add(Row(Lbl("Data (hex bytes)"), _txtData));
        stack.Controls.Add(Row(_lblHint));
        stack.Controls.Add(Row(Lbl("Packet"), _txtPreview, _btnSend));
        stack.Controls.Add(Row(_lblInfo));
        return Group("Packet", stack);
    }

    private GroupBox BuildLogGroup()
    {
        var toolbar = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
        };
        toolbar.Controls.Add(_btnClear);
        toolbar.Controls.Add(_chkAutoScroll);

        var host = new Panel { Dock = DockStyle.Fill };
        host.Controls.Add(_rtbLog);
        host.Controls.Add(toolbar);

        var grp = new GroupBox { Text = "Traffic log", Dock = DockStyle.Fill };
        grp.Controls.Add(host);
        return grp;
    }

    private void WireEvents()
    {
        _cboPreset.SelectedIndexChanged += (_, _) => ApplyPreset();
        foreach (var tb in new[] { _txtDest, _txtSrc, _txtBsn, _txtCmd1, _txtCmd2, _txtData })
            tb.TextChanged += (_, _) => UpdatePreview();

        _btnOpen.Click += (_, _) => ToggleSocket();
        _btnSend.Click += (_, _) => Send();
        _btnClear.Click += (_, _) => _rtbLog.Clear();
        FormClosing += (_, _) => CloseSocket();
    }

    // ---- Preset handling ---------------------------------------------------

    private void ApplyPreset()
    {
        if (_cboPreset.SelectedItem is not CommandPreset preset)
            return;

        _lblHint.Text = preset.Hint;
        if (preset.DataLength < 0)
        {
            UpdatePreview();
            return;
        }

        _suppressPreview = true;
        _txtCmd1.Text = preset.Cmd1.ToString("X2");
        _txtCmd2.Text = preset.Cmd2.ToString("X2");
        _txtData.Text = preset.DataLength == 0
            ? string.Empty
            : string.Join(' ', Enumerable.Repeat("00", preset.DataLength));
        _suppressPreview = false;
        UpdatePreview();
    }

    // ---- Live preview ------------------------------------------------------

    private void UpdatePreview()
    {
        if (_suppressPreview)
            return;

        try
        {
            uint dest = ParseAddress(_txtDest.Text, "Destination");
            uint src = ParseAddress(_txtSrc.Text, "Source");
            byte bsn = ParseByte(_txtBsn.Text, "BSN");
            byte cmd1 = ParseByte(_txtCmd1.Text, "Command 1");
            byte cmd2 = ParseByte(_txtCmd2.Text, "Command 2");
            byte[] data = ParseHexBytes(_txtData.Text);

            _lblDataLen.Text = data.Length.ToString();
            if (data.Length > P4Protocol.MaxData)
                throw new FormatException($"Data is {data.Length} bytes; the protocol allows at most {P4Protocol.MaxData}.");

            byte[] packet = P4Protocol.Build(dest, src, bsn, cmd1, cmd2, data);
            _currentPacket = packet;

            ushort checksum = P4Protocol.Checksum(packet.AsSpan(1, packet.Length - 4));
            _txtPreview.ForeColor = Color.Black;
            _txtPreview.Text = FormatHex(packet);
            _lblInfo.ForeColor = Color.DimGray;
            _lblInfo.Text = $"{packet.Length} bytes  •  checksum 0x{checksum:X4}  •  {P4Protocol.CommandName(cmd1, cmd2)}";
            _btnSend.Enabled = true;
        }
        catch (Exception ex)
        {
            _currentPacket = null;
            _txtPreview.ForeColor = Color.Firebrick;
            _txtPreview.Text = "⚠ " + ex.Message;
            _lblInfo.Text = string.Empty;
            _btnSend.Enabled = false;
        }
    }

    // ---- Networking --------------------------------------------------------

    private void ToggleSocket()
    {
        if (_udp is null)
            OpenSocket();
        else
            CloseSocket();
    }

    private bool OpenSocket()
    {
        try
        {
            int localPort = (int)_numLocalPort.Value;
            var udp = new UdpClient(new IPEndPoint(IPAddress.Any, localPort));
            _udp = udp;
            _cts = new CancellationTokenSource();
            _ = ReceiveLoopAsync(udp, _cts.Token);

            int bound = ((IPEndPoint)udp.Client.LocalEndPoint!).Port;
            _btnOpen.Text = "Close";
            _numLocalPort.Enabled = false;
            _lblStatus.ForeColor = Color.SeaGreen;
            _lblStatus.Text = $"Listening on 0.0.0.0:{bound}";
            return true;
        }
        catch (Exception ex)
        {
            _udp = null;
            _lblStatus.ForeColor = Color.Firebrick;
            _lblStatus.Text = "Open failed: " + ex.Message;
            return false;
        }
    }

    private void CloseSocket()
    {
        try { _cts?.Cancel(); } catch { /* ignore */ }
        try { _udp?.Close(); } catch { /* ignore */ }
        _udp?.Dispose();
        _udp = null;
        _cts?.Dispose();
        _cts = null;

        if (!IsDisposed)
        {
            _btnOpen.Text = "Open / Listen";
            _numLocalPort.Enabled = true;
            _lblStatus.ForeColor = Color.DimGray;
            _lblStatus.Text = "Closed.";
        }
    }

    private void Send()
    {
        if (_currentPacket is null)
        {
            MessageBox.Show(this, "Fix the packet fields before sending.", "Invalid packet",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        IPEndPoint remote;
        try
        {
            remote = new IPEndPoint(IPAddress.Parse(_txtIp.Text.Trim()), (int)_numPort.Value);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Invalid target IP: " + ex.Message, "Invalid target",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_udp is null && !OpenSocket())
            return;

        try
        {
            byte[] packet = _currentPacket;
            _udp!.Send(packet, packet.Length, remote);
            LogPacket("TX →", remote, packet);
        }
        catch (Exception ex)
        {
            AppendLog($"TX error: {ex.Message}{Environment.NewLine}", Color.Firebrick, bold: true);
        }
    }

    private async Task ReceiveLoopAsync(UdpClient udp, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult result = await udp.ReceiveAsync(token);
                byte[] buffer = result.Buffer;
                IPEndPoint from = result.RemoteEndPoint;
                if (IsDisposed)
                    return;
                BeginInvoke(() => LogPacket("RX ←", from, buffer));
            }
        }
        catch (OperationCanceledException) { /* socket closing */ }
        catch (ObjectDisposedException) { /* socket closed */ }
        catch (Exception ex)
        {
            if (!IsDisposed)
                BeginInvoke(() => AppendLog($"RX error: {ex.Message}{Environment.NewLine}", Color.Firebrick, bold: true));
        }
    }

    // ---- Logging -----------------------------------------------------------

    private void LogPacket(string direction, IPEndPoint endpoint, byte[] buffer)
    {
        bool tx = direction.StartsWith("TX", StringComparison.Ordinal);
        string stamp = DateTime.Now.ToString("HH:mm:ss.fff");

        if (P4Protocol.TryParse(buffer, out ParsedPacket p, out string error))
        {
            string chk = p.ChecksumValid
                ? "chk OK"
                : $"chk BAD (got 0x{p.ReceivedChecksum:X4}, calc 0x{p.ComputedChecksum:X4})";
            string header = $"{stamp}  {direction} {endpoint}  {buffer.Length} bytes  " +
                            $"dest {p.Destination} src {p.Source}  {P4Protocol.CommandName(p.Command1, p.Command2)}  {chk}";
            Color color = tx ? Color.RoyalBlue : (p.ChecksumValid ? Color.SeaGreen : Color.Firebrick);
            AppendLog(header + Environment.NewLine, color, bold: true);

            if (!string.IsNullOrEmpty(error))
                AppendLog("   " + error + Environment.NewLine, Color.DarkGoldenrod);

            string? decoded = P4Protocol.DecodePayload(p);
            if (decoded is not null)
                AppendLog("   " + decoded + Environment.NewLine, Color.DimGray);
        }
        else
        {
            AppendLog($"{stamp}  {direction} {endpoint}  {buffer.Length} bytes  (unparsed: {error})" + Environment.NewLine,
                tx ? Color.RoyalBlue : Color.Firebrick, bold: true);
        }

        AppendLog("   " + FormatHex(buffer) + Environment.NewLine, Color.Black);
    }

    private void AppendLog(string text, Color color, bool bold = false)
    {
        _rtbLog.SelectionStart = _rtbLog.TextLength;
        _rtbLog.SelectionLength = 0;
        _rtbLog.SelectionColor = color;
        _rtbLog.SelectionFont = new Font(_mono, bold ? FontStyle.Bold : FontStyle.Regular);
        _rtbLog.AppendText(text);
        _rtbLog.SelectionColor = _rtbLog.ForeColor;

        if (_chkAutoScroll.Checked)
        {
            _rtbLog.SelectionStart = _rtbLog.TextLength;
            _rtbLog.ScrollToCaret();
        }
    }

    // ---- Parsing helpers ---------------------------------------------------

    private static uint ParseAddress(string text, string field)
    {
        text = text.Trim();
        if (text.Length == 0)
            return 0;
        try
        {
            return text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? Convert.ToUInt32(text[2..], 16)
                : uint.Parse(text, CultureInfo.InvariantCulture);
        }
        catch
        {
            throw new FormatException($"{field} '{text}' is not a valid 32-bit address (use decimal or 0x hex).");
        }
    }

    private static byte ParseByte(string text, string field)
    {
        text = text.Trim();
        if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            text = text[2..];
        if (text.Length == 0)
            return 0;
        if (byte.TryParse(text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte value))
            return value;
        throw new FormatException($"{field} '{text}' is not a valid hex byte (00–FF).");
    }

    private static byte[] ParseHexBytes(string text)
    {
        text = text.Trim();
        if (text.Length == 0)
            return Array.Empty<byte>();

        string[] tokens = text.Split(new[] { ' ', '\t', '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries);

        // A single run of contiguous hex digits ("0203FF") -> split into byte pairs.
        if (tokens.Length == 1 && tokens[0].Length > 2)
        {
            string run = tokens[0];
            if (run.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                run = run[2..];
            if (run.Length % 2 != 0)
                throw new FormatException($"Hex string '{tokens[0]}' has an odd number of digits.");
            var bytes = new byte[run.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = ParseByte(run.Substring(i * 2, 2), "Data");
            return bytes;
        }

        return tokens.Select(t => ParseByte(t, "Data")).ToArray();
    }

    private static string FormatHex(byte[] data) =>
        string.Join(' ', data.Select(b => b.ToString("X2")));

    // ---- Small UI factory helpers -----------------------------------------

    private static FlowLayoutPanel NewStack() => new()
    {
        Dock = DockStyle.Fill,
        FlowDirection = FlowDirection.TopDown,
        WrapContents = false,
        AutoScroll = false,
    };

    private static FlowLayoutPanel Row(params Control[] controls)
    {
        var row = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 2),
        };
        row.Controls.AddRange(controls);
        return row;
    }

    private static Label Lbl(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Margin = new Padding(3, 7, 6, 0),
    };

    private static GroupBox Group(string title, Control content)
    {
        var grp = new GroupBox { Text = title, Dock = DockStyle.Fill, Padding = new Padding(6, 4, 6, 4) };
        grp.Controls.Add(content);
        return grp;
    }
}
