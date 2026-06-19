namespace P4ProtocolBuilder;

/// <summary>
/// Encoding / decoding for the Progeny P3/P4 UDP protocol (Doc 4002, v4.68).
///
/// Wire layout (variable length, 16 + DataLength bytes total):
///   [0]      STX (0x02)
///   [1..4]   Destination address, MSB first
///   [5..8]   Source address, MSB first
///   [9]      Block Sequence Number (BSN, default 0x01)
///   [10]     Command 1 (command group)
///   [11]     Command 2 (command number)
///   [12]     Data length (0..16)
///   [13..]   Data bytes (DataLength of them)
///   [n]      Checksum LSB
///   [n+1]    Checksum MSB
///   [n+2]    ETX (0x03)
///
/// Checksum = 16-bit sum of every byte from Destination[0] through the last data byte.
/// </summary>
public static class P4Protocol
{
    public const byte STX = 0x02;
    public const byte ETX = 0x03;
    public const int MaxData = 16;

    /// <summary>Sum of Destination[0]..last data byte, masked to 16 bits.</summary>
    public static ushort Checksum(ReadOnlySpan<byte> body)
    {
        int sum = 0;
        foreach (byte b in body)
            sum += b;
        return (ushort)(sum & 0xFFFF);
    }

    /// <summary>Builds a complete wire packet (STX..ETX) from the supplied fields.</summary>
    public static byte[] Build(uint dest, uint src, byte bsn, byte cmd1, byte cmd2, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length > MaxData)
            throw new ArgumentException($"Data length cannot exceed {MaxData} bytes (got {data.Length}).");

        // body = the checksummed region (destination .. last data byte)
        byte[] body = new byte[12 + data.Length];
        body[0] = (byte)(dest >> 24);
        body[1] = (byte)(dest >> 16);
        body[2] = (byte)(dest >> 8);
        body[3] = (byte)dest;
        body[4] = (byte)(src >> 24);
        body[5] = (byte)(src >> 16);
        body[6] = (byte)(src >> 8);
        body[7] = (byte)src;
        body[8] = bsn;
        body[9] = cmd1;
        body[10] = cmd2;
        body[11] = (byte)data.Length;
        Array.Copy(data, 0, body, 12, data.Length);

        ushort checksum = Checksum(body);

        byte[] packet = new byte[1 + body.Length + 3];
        packet[0] = STX;
        Array.Copy(body, 0, packet, 1, body.Length);
        int i = 1 + body.Length;
        packet[i++] = (byte)(checksum & 0xFF); // LSB
        packet[i++] = (byte)(checksum >> 8);    // MSB
        packet[i] = ETX;
        return packet;
    }

    /// <summary>Attempts to decode a received packet for display in the log.</summary>
    public static bool TryParse(byte[] buffer, out ParsedPacket packet, out string error)
    {
        packet = default!;
        if (buffer.Length < 16)
        {
            error = $"Too short: {buffer.Length} bytes (minimum 16).";
            return false;
        }

        int dataLen = buffer[12];
        int expectedLen = 16 + dataLen;

        var p = new ParsedPacket
        {
            Stx = buffer[0],
            Destination = (uint)((buffer[1] << 24) | (buffer[2] << 16) | (buffer[3] << 8) | buffer[4]),
            Source = (uint)((buffer[5] << 24) | (buffer[6] << 16) | (buffer[7] << 8) | buffer[8]),
            Bsn = buffer[9],
            Command1 = buffer[10],
            Command2 = buffer[11],
            DataLength = (byte)dataLen,
        };

        // Pull whatever data bytes are actually present, even on a length mismatch.
        int availableData = Math.Min(dataLen, Math.Max(0, buffer.Length - 16));
        p.Data = new byte[availableData];
        Array.Copy(buffer, 13, p.Data, 0, availableData);

        // Checksum covers destination..last data byte (indices 1 .. 12 + dataLen).
        int bodyLen = Math.Min(12 + dataLen, buffer.Length - 1);
        ushort computed = Checksum(buffer.AsSpan(1, bodyLen));
        p.ComputedChecksum = computed;

        if (buffer.Length >= expectedLen)
        {
            p.ReceivedChecksum = (ushort)(buffer[14 + dataLen] | (buffer[15 + dataLen] << 8));
            p.Etx = buffer[expectedLen - 1];
            p.ChecksumValid = p.ReceivedChecksum == computed;
            p.FramingValid = p.Stx == STX && p.Etx == ETX && buffer.Length == expectedLen;
        }

        packet = p;
        error = buffer.Length == expectedLen
            ? string.Empty
            : $"Length {buffer.Length} != expected {expectedLen} for data length {dataLen}.";
        return true;
    }

    /// <summary>Friendly name for a command group/number pair, e.g. "B,1 Ping".</summary>
    public static string CommandName(byte cmd1, byte cmd2)
    {
        foreach (var preset in CommandPreset.All)
            if (preset.DataLength >= 0 && preset.Cmd1 == cmd1 && preset.Cmd2 == cmd2)
                return preset.Name;
        return $"{cmd1:X},{cmd2:X} (unknown)";
    }

    /// <summary>Event Type appendix (used to annotate D,3 Event Log responses).</summary>
    public static readonly IReadOnlyDictionary<byte, string> EventTypes = new Dictionary<byte, string>
    {
        [0] = "E_CARD_OK",
        [1] = "E_INVALID_CARD",
        [2] = "E_HACKER",
        [3] = "E_DOOR_PC_RELEASE",
        [4] = "E_CODE_DURESS",
        [5] = "E_DOOR_FORCED",
        [6] = "E_PDO",
        [7] = "E_FIRE_ALARM",
        [8] = "E_INTRUDER",
        [15] = "E_APB_FAILURE",
        [16] = "E_CARD_AND_PIN_OK",
        [17] = "E_CARD_OTL",
        [18] = "E_UNKNOWN_CARD",
        [19] = "E_DOOR_PC_CLOSE",
        [20] = "E_INTRUDER_RESET",
        [22] = "E_RQE",
        [23] = "E_CODE_OK",
        [24] = "E_FIRE_RESET",
        [25] = "E_CARD_EXPIRED",
        [26] = "E_INVALID_PIN",
        [29] = "E_DOOR_PC_OPEN",
        [30] = "E_TZ_DOOR_OPEN",
        [31] = "E_INVALID_CODE",
        [36] = "E_MENU_ENGINEER",
        [37] = "E_MENU_USER",
        [38] = "E_TZ_DOOR_CLOSE",
        [40] = "E_FIRE_FAULT",
        [41] = "E_INTRUDER_FAULT",
        [42] = "E_POWER_DOWN",
        [43] = "E_POWER_UP",
        [44] = "E_RANDOM_SEARCH",
        [45] = "E_READER_ERROR",
        [51] = "E_PREMATURE_CARD",
        [52] = "E_SUPPLY_LOW_VOLTS",
        [53] = "E_SUPPLY_HIGH_VOLTS",
        [102] = "E_DOOR_PC_OPEN_B",
        [103] = "E_DOOR_PC_CLOSE_B",
    };

    /// <summary>Short human-readable decode for responses that carry structured data.</summary>
    public static string? DecodePayload(ParsedPacket p)
    {
        // B,2 Ping response
        if (p.Command1 == 0x0B && p.Command2 == 0x02 && p.Data.Length >= 16)
        {
            byte[] d = p.Data;
            return $"Ping reply: v{d[13]}.{d[14]}, supply {d[15] / 10.0:0.0}V, " +
                   $"time {d[11]:00}:{d[10]:00}:{d[9]:00} {d[6]:00}/{d[7]:00}/{d[8]:00}, " +
                   $"status1=0x{d[4]:X2} status2=0x{d[5]:X2}, unread logs {d[3]:X2}{d[2]:X2}";
        }

        // D,3 Event Log response
        if (p.Command1 == 0x0D && p.Command2 == 0x03 && p.Data.Length >= 16)
        {
            byte[] d = p.Data;
            string ev = EventTypes.TryGetValue(d[14], out var name) ? name : $"0x{d[14]:X2}";
            byte reader = d[12];
            string readerName = reader == 1 ? "A" : reader == 2 ? "B" : $"0x{reader:X2}";
            return $"Event: {ev}, reader {readerName}, " +
                   $"time {d[2]:00}:{d[1]:00}:{d[0]:00} {d[3]:00}/{d[4]:00}/{d[5]:00}";
        }

        return null;
    }
}

/// <summary>Decoded view of a received packet.</summary>
public sealed class ParsedPacket
{
    public byte Stx { get; set; }
    public uint Destination { get; set; }
    public uint Source { get; set; }
    public byte Bsn { get; set; }
    public byte Command1 { get; set; }
    public byte Command2 { get; set; }
    public byte DataLength { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();
    public ushort ReceivedChecksum { get; set; }
    public ushort ComputedChecksum { get; set; }
    public byte Etx { get; set; }
    public bool ChecksumValid { get; set; }
    public bool FramingValid { get; set; }
}

/// <summary>A documented command, used to pre-fill the builder fields.</summary>
public sealed record CommandPreset(string Name, byte Cmd1, byte Cmd2, int DataLength, string Hint)
{
    public override string ToString() => Name;

    public static readonly IReadOnlyList<CommandPreset> All = new[]
    {
        new CommandPreset("— Custom / Raw —", 0, 0, -1,
            "Enter Command 1, Command 2 and the data bytes manually."),
        new CommandPreset("1,1 Set Controller Address", 0x01, 0x01, 4,
            "Data = the new 4-byte controller address (MSB..LSB)."),
        new CommandPreset("1,4 Engineer Functions", 0x01, 0x04, 12,
            "D0 selects: 0x16 Clear Event Log, 0x17 Clear Card Data, 0x98 Clear Door Access Code."),
        new CommandPreset("1,5 Users Pack of Functions", 0x01, 0x05, 16,
            "D0: 0x00 Set Door Access Code, 0x14 Enrol Bio, 0x15 Delete Bio."),
        new CommandPreset("1,6 Engineers Pack of Functions", 0x01, 0x06, 16,
            "D0 selects sub-block 0x00 / 0x01 / 0x02 (reader/lock/door settings)."),
        new CommandPreset("1,7 Input Control", 0x01, 0x07, 2,
            "Byte0 bitfield (RQE / Reader B / Reader A / Remote Validation); Byte1 remote-validation timeout."),
        new CommandPreset("3,1 Delete Card Data", 0x03, 0x01, 14,
            "D0..D7 = 0x00; D8..D13 = card number C1 C2 B1 B2 A1 A2."),
        new CommandPreset("3,2 Set Card Data", 0x03, 0x02, 16,
            "PIN, access levels A/B, valid-from/to dates, card number, then 00 00. See spec 3,2."),
        new CommandPreset("5,1 Time Zone Setting", 0x05, 0x01, 8,
            "TZ element#, start H, start M, end H, end M, calendar#, week-map, TZ pointer."),
        new CommandPreset("5,2 Calendar Setting", 0x05, 0x02, 14,
            "Calendar#, quarter#, then 12 day-bitmap bytes (one bit per day)."),
        new CommandPreset("5,4 Trigger Channel A/B", 0x05, 0x04, 3,
            "D0 channel (01=Lock A, 02=Lock B); D1 mode (00 timed, 01 open forever, 02 close); D2 seconds."),
        new CommandPreset("5,5 Access Level (All Access)", 0x05, 0x05, 16,
            "D0=0x00 for All Access; D1..D15 = TZ pointer per access level."),
        new CommandPreset("7,1 Set Controller Time", 0x07, 0x01, 6,
            "HH MM SS as digit pairs, e.g. 1 5 3 0 4 7 => 15:30:47."),
        new CommandPreset("7,2 Set Controller Date", 0x07, 0x02, 9,
            "DD MM YY then day-of-week, as digit pairs, e.g. 1 4 1 0 2 0 2 3 3 => 14/10/2023 Tue."),
        new CommandPreset("9,B Reset Alarm", 0x09, 0x0B, 0,
            "Reset latched alarms on the controller."),
        new CommandPreset("9,E Ext. Validation Request", 0x09, 0x0E, 15,
            "Controller->PC: card number (D0..D5) + reader status (D6: 01=A, 02=B)."),
        new CommandPreset("9,F External Validation Reply", 0x09, 0x0F, 1,
            "PC->Controller: 0=Card OK, 1=Card Invalid, 6=Card Unknown."),
        new CommandPreset("B,1 Ping", 0x0B, 0x01, 0,
            "Request controller status (controller answers with B,2)."),
        new CommandPreset("B,3 Get Reader Information", 0x0B, 0x03, 0,
            "Request reader information."),
        new CommandPreset("D,1 Event Log Request (unread)", 0x0D, 0x01, 2,
            "Read oldest unread event log. D0/D1 = number of records (digit pairs)."),
        new CommandPreset("D,2 Event Log Acknowledge", 0x0D, 0x02, 0,
            "Acknowledge / mark the last event log as read."),
        new CommandPreset("D,4 Latest Event Log Request", 0x0D, 0x04, 0,
            "Request the latest event log."),
    };
}
