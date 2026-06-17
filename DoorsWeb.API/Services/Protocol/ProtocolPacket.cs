namespace DoorsWeb.API.Services.Protocol
{
    /// <summary>
    /// A controller communication packet.
    ///
    /// On-wire layout (variable length, 16..32 bytes):
    ///   [0]      STX (0x02)
    ///   [1..4]   Destination address, big-endian (Destination 1 = MSB .. Destination 4 = LSB)
    ///   [5..8]   Source address, big-endian
    ///   [9]      Block Sequence Number (BSN)
    ///   [10]     Command Group   (Command 1)
    ///   [11]     Command Number  (Command 2)
    ///   [12]     Data Length (n), 0..16
    ///   [13..]   Data, n bytes
    ///   [13+n]   Checksum LSB (Checksum 1)
    ///   [14+n]   Checksum MSB (Checksum 2)
    ///   [15+n]   ETX (0x03)
    ///
    /// Checksum = 16-bit sum of bytes from Destination 1 through the last data byte.
    /// </summary>
    public sealed class ProtocolPacket
    {
        public const byte Stx = 0x02;
        public const byte Etx = 0x03;
        public const int MaxDataLength = 16;

        /// <summary>Framing overhead with zero data bytes (STX + 8 address + BSN + 2 command + length + 2 checksum + ETX).</summary>
        private const int FrameOverhead = 16;

        public uint DestinationAddress { get; set; }
        public uint SourceAddress { get; set; }
        public byte BlockSequenceNumber { get; set; } = 0x01;
        public byte CommandGroup { get; set; }
        public byte CommandNumber { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();

        /// <summary>Serializes this packet to its on-wire byte representation, computing the checksum.</summary>
        public byte[] ToBytes()
        {
            if (Data.Length > MaxDataLength)
                throw new ArgumentOutOfRangeException(nameof(Data), $"Data length {Data.Length} exceeds {MaxDataLength} bytes.");

            int n = Data.Length;
            var buffer = new byte[FrameOverhead + n];
            int i = 0;

            buffer[i++] = Stx;
            buffer[i++] = (byte)(DestinationAddress >> 24);
            buffer[i++] = (byte)(DestinationAddress >> 16);
            buffer[i++] = (byte)(DestinationAddress >> 8);
            buffer[i++] = (byte)DestinationAddress;
            buffer[i++] = (byte)(SourceAddress >> 24);
            buffer[i++] = (byte)(SourceAddress >> 16);
            buffer[i++] = (byte)(SourceAddress >> 8);
            buffer[i++] = (byte)SourceAddress;
            buffer[i++] = BlockSequenceNumber;
            buffer[i++] = CommandGroup;
            buffer[i++] = CommandNumber;
            buffer[i++] = (byte)n;
            Array.Copy(Data, 0, buffer, i, n);
            i += n;

            ushort checksum = ComputeChecksum(buffer, 1, 12 + n); // Destination 1 .. last data byte
            buffer[i++] = (byte)(checksum & 0xFF);        // LSB
            buffer[i++] = (byte)((checksum >> 8) & 0xFF); // MSB
            buffer[i++] = Etx;

            return buffer;
        }

        /// <summary>Attempts to parse a received datagram, validating STX, length, checksum and ETX.</summary>
        public static bool TryParse(ReadOnlySpan<byte> bytes, out ProtocolPacket? packet, out string? error)
        {
            packet = null;
            error = null;

            if (bytes.Length < FrameOverhead) { error = $"Frame too short ({bytes.Length} bytes)."; return false; }
            if (bytes[0] != Stx) { error = $"Missing STX (got 0x{bytes[0]:X2})."; return false; }

            int n = bytes[12];
            if (n > MaxDataLength) { error = $"Data length {n} exceeds {MaxDataLength}."; return false; }

            int expectedLength = FrameOverhead + n;
            if (bytes.Length < expectedLength) { error = $"Frame length {bytes.Length} is shorter than expected {expectedLength}."; return false; }

            int checksumIndex = 13 + n;
            if (bytes[checksumIndex + 2] != Etx) { error = $"Missing ETX (got 0x{bytes[checksumIndex + 2]:X2})."; return false; }

            ushort received = (ushort)(bytes[checksumIndex] | (bytes[checksumIndex + 1] << 8));
            ushort computed = ComputeChecksum(bytes, 1, 12 + n);
            if (received != computed) { error = $"Checksum mismatch (got 0x{received:X4}, expected 0x{computed:X4})."; return false; }

            packet = new ProtocolPacket
            {
                DestinationAddress = ReadUInt32(bytes, 1),
                SourceAddress = ReadUInt32(bytes, 5),
                BlockSequenceNumber = bytes[9],
                CommandGroup = bytes[10],
                CommandNumber = bytes[11],
                Data = bytes.Slice(13, n).ToArray()
            };
            return true;
        }

        private static ushort ComputeChecksum(ReadOnlySpan<byte> bytes, int start, int count)
        {
            int sum = 0;
            for (int i = start; i < start + count; i++) sum += bytes[i];
            return (ushort)(sum & 0xFFFF);
        }

        private static uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset) =>
            (uint)((bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3]);

        public override string ToString() =>
            $"Src=0x{SourceAddress:X8} Dst=0x{DestinationAddress:X8} BSN={BlockSequenceNumber} " +
            $"Cmd={CommandGroup}/{CommandNumber} Len={Data.Length} Data=[{Convert.ToHexString(Data)}]";
    }
}
