using System.Buffers.Binary;
using System.Text;

namespace DoorsWeb.API.Legacy.Adtg
{
    /// <summary>
    /// A single column of a persisted ADO recordset, as described by the ADTG metadata.
    /// </summary>
    public sealed class AdtgColumn
    {
        public required int Ordinal { get; init; }
        public required string Name { get; init; }

        /// <summary>OLE DB type code (DBTYPE) as stored in the recordset metadata.</summary>
        public required short Type { get; init; }

        /// <summary>Declared maximum length in characters (for variable-length string columns).</summary>
        public required int MaxLen { get; init; }

        /// <summary>DBCOLUMNFLAGS. Bit 0x20 (DBCOLUMNFLAGS_ISNULLABLE) marks a nullable column.</summary>
        public required uint Flags { get; init; }

        public bool IsNullable => (Flags & 0x20u) != 0;
    }

    /// <summary>
    /// Cross-platform reader for the ADTG ("Advanced Data TableGram", magic "TG!") binary format
    /// produced by ADO's <c>Recordset.Save(..., adPersistADTG)</c> — the on-disk shape of the
    /// legacy DoorsClient <c>.rs</c> table dumps.
    ///
    /// This is a managed re-implementation (no COM / no Windows dependency) so it runs inside the
    /// Linux API container. It was reverse-engineered and validated cell-by-cell against an
    /// ADODB.Recordset ground-truth dump for every table in the production backup, including the
    /// ~2M-row T_Events table.
    ///
    /// Layout (little-endian throughout):
    ///   [preamble + per-column descriptors][row data ...][0x0F terminator]
    /// Each descriptor begins with <c>06 &lt;len:2&gt; F2 01 00</c>; the column name appears twice in
    /// the body, and the type/maxlen/precision/scale/flags tuple follows the second occurrence.
    /// Each row begins with token <c>0x07</c>, then a null bitmap covering only the nullable columns
    /// (MSB-first, 1 = present / 0 = null, trailing pad bits = 1), then the present column values in
    /// ordinal order. The row section ends at token <c>0x0F</c>.
    /// </summary>
    public sealed class AdtgRecordsetReader : IDisposable
    {
        // OLE DB (DBTYPE) constants seen in these backups.
        private const short AdSmallInt = 2;
        private const short AdInt = 3;
        private const short AdReal = 4;
        private const short AdDate = 7;
        private const short AdBool = 11;
        private const short AdDbTimeStamp = 135;
        private const short AdWChar = 130;
        private const short AdVarWChar = 202;
        private const short AdChar = 129;
        private const short AdVarChar = 200;

        // Metadata lives at the very start of the file; 2 MB is far more than enough for even the
        // widest table (T_Doors, 83 columns) while keeping the header read bounded.
        private const int MaxHeaderBytes = 2 * 1024 * 1024;

        private readonly Stream _stream;
        private readonly bool _ownsStream;
        private readonly Stream _buffered;
        private readonly int _bitmapBytes;
        private readonly int[] _nullableIndexes;
        private readonly byte[] _scratch = new byte[16];
        private readonly byte[] _bitmap;
        private bool _disposed;

        public IReadOnlyList<AdtgColumn> Columns { get; }

        private AdtgRecordsetReader(Stream stream, bool ownsStream, List<AdtgColumn> columns, long rowsStart)
        {
            _stream = stream;
            _ownsStream = ownsStream;
            Columns = columns;

            var nullable = new List<int>();
            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].IsNullable) nullable.Add(i);
            }
            _nullableIndexes = nullable.ToArray();
            _bitmapBytes = _nullableIndexes.Length > 0 ? (_nullableIndexes.Length + 7) / 8 : 0;
            _bitmap = new byte[Math.Max(1, _bitmapBytes)];

            _stream.Seek(rowsStart, SeekOrigin.Begin);
            _buffered = new BufferedStream(_stream, 1 << 16);
        }

        public static AdtgRecordsetReader Open(string path)
            => Open(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read), ownsStream: true);

        public static AdtgRecordsetReader Open(Stream stream, bool ownsStream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("ADTG parsing requires a seekable stream.", nameof(stream));

            long length = stream.Length;
            int headerLen = (int)Math.Min(length, MaxHeaderBytes);
            var header = new byte[headerLen];
            stream.Seek(0, SeekOrigin.Begin);
            ReadExactly(stream, header, headerLen);

            var (columns, rowsStart) = ParseMetadata(header);
            return new AdtgRecordsetReader(stream, ownsStream, columns, rowsStart);
        }

        /// <summary>Maps an ADTG column to the CLR type used when materializing its values.</summary>
        public static Type ClrTypeOf(AdtgColumn column) => column.Type switch
        {
            AdInt => typeof(int),
            AdSmallInt => typeof(short),
            AdBool => typeof(bool),
            AdReal => typeof(float),
            AdDate or AdDbTimeStamp => typeof(DateTime),
            _ => typeof(string),
        };

        private static (List<AdtgColumn> columns, long rowsStart) ParseMetadata(byte[] b)
        {
            int n = b.Length;
            var descriptorOffsets = new List<int>();
            int j = 0;
            while (j < n - 6)
            {
                if (b[j] == 0x06 && b[j + 3] == 0xF2 && b[j + 4] == 0x01 && b[j + 5] == 0x00)
                {
                    descriptorOffsets.Add(j);
                    int dlen = ReadU16(b, j + 1);
                    j = j + 1 + 2 + dlen;
                }
                else
                {
                    j++;
                }
            }

            if (descriptorOffsets.Count == 0)
                throw new InvalidDataException("No ADTG column descriptors were found; not a recognized .rs recordset.");

            var columns = new List<AdtgColumn>(descriptorOffsets.Count);
            long rowsStart = 0;
            foreach (int d in descriptorOffsets)
            {
                int dlen = ReadU16(b, d + 1);
                int body = d + 3;                 // points at F2 01 00
                int ordinal = ReadU16(b, body + 3);
                int nameLen = ReadU16(b, body + 5);
                string name = Encoding.Unicode.GetString(b, body + 7, nameLen * 2);

                // The column name appears twice in the descriptor body; the type tuple follows the
                // second occurrence.
                byte[] nameBytes = Encoding.Unicode.GetBytes(name);
                int first = IndexOf(b, nameBytes, body);
                int second = IndexOf(b, nameBytes, first + 1);
                if (first < 0 || second < 0)
                    throw new InvalidDataException($"Malformed ADTG descriptor for column '{name}'.");

                int after = second + nameBytes.Length;
                short type = (short)ReadU16(b, after);
                uint maxLen = ReadU32(b, after + 2);
                // precision = ReadU32(b, after + 6); scale = ReadU32(b, after + 10);  (unused)
                uint flags = ReadU32(b, after + 14);

                columns.Add(new AdtgColumn
                {
                    Ordinal = ordinal,
                    Name = name,
                    Type = type,
                    MaxLen = (int)maxLen,
                    Flags = flags,
                });

                rowsStart = d + 1 + 2 + dlen;
            }

            columns.Sort((x, y) => x.Ordinal.CompareTo(y.Ordinal));
            return (columns, rowsStart);
        }

        /// <summary>
        /// Streams every row of the recordset. Each yielded array holds one value per column in
        /// ordinal order; a null entry represents a SQL NULL.
        /// </summary>
        public IEnumerable<object?[]> ReadRows()
        {
            int columnCount = Columns.Count;
            while (true)
            {
                int token = _buffered.ReadByte();
                if (token < 0 || token == 0x0F) yield break;
                if (token != 0x07) continue;

                var present = new bool[columnCount];
                for (int c = 0; c < columnCount; c++) present[c] = true;

                if (_bitmapBytes > 0)
                {
                    FillInto(_bitmap, _bitmapBytes);
                    for (int idx = 0; idx < _nullableIndexes.Length; idx++)
                    {
                        byte bm = _bitmap[idx >> 3];
                        int bit = 7 - (idx & 7);
                        present[_nullableIndexes[idx]] = (bm & (1 << bit)) != 0;
                    }
                }

                var row = new object?[columnCount];
                for (int c = 0; c < columnCount; c++)
                {
                    if (!present[c])
                    {
                        row[c] = null;
                        continue;
                    }
                    row[c] = ReadValue(Columns[c]);
                }
                yield return row;
            }
        }

        private object ReadValue(AdtgColumn column)
        {
            switch (column.Type)
            {
                case AdInt:
                    Fill(4);
                    return BinaryPrimitives.ReadInt32LittleEndian(_scratch);
                case AdSmallInt:
                    Fill(2);
                    return BinaryPrimitives.ReadInt16LittleEndian(_scratch);
                case AdBool:
                    Fill(2);
                    return BinaryPrimitives.ReadUInt16LittleEndian(_scratch) != 0;
                case AdReal:
                    Fill(4);
                    return BinaryPrimitives.ReadSingleLittleEndian(_scratch);
                case AdDate:
                case AdDbTimeStamp:
                {
                    Fill(16);
                    short year = BinaryPrimitives.ReadInt16LittleEndian(_scratch.AsSpan(0, 2));
                    ushort month = BinaryPrimitives.ReadUInt16LittleEndian(_scratch.AsSpan(2, 2));
                    ushort day = BinaryPrimitives.ReadUInt16LittleEndian(_scratch.AsSpan(4, 2));
                    ushort hour = BinaryPrimitives.ReadUInt16LittleEndian(_scratch.AsSpan(6, 2));
                    ushort minute = BinaryPrimitives.ReadUInt16LittleEndian(_scratch.AsSpan(8, 2));
                    ushort second = BinaryPrimitives.ReadUInt16LittleEndian(_scratch.AsSpan(10, 2));
                    uint fractionNanos = BinaryPrimitives.ReadUInt32LittleEndian(_scratch.AsSpan(12, 4));
                    var dt = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                    return dt.AddTicks(fractionNanos / 100); // 1 tick = 100 ns
                }
                default:
                {
                    // Variable-length string: prefix is 1 byte when the declared byte size fits in a
                    // byte, otherwise a 4-byte length.
                    int bytesPerChar = BytesPerChar(column.Type);
                    long maxBytes = (long)column.MaxLen * bytesPerChar;
                    int len;
                    if (maxBytes <= 255)
                    {
                        Fill(1);
                        len = _scratch[0];
                    }
                    else
                    {
                        Fill(4);
                        len = (int)BinaryPrimitives.ReadUInt32LittleEndian(_scratch);
                    }

                    if (len == 0) return string.Empty;
                    var raw = new byte[len];
                    FillInto(raw, len);
                    return bytesPerChar == 2
                        ? Encoding.Unicode.GetString(raw)
                        : Encoding.Latin1.GetString(raw);
                }
            }
        }

        private static int BytesPerChar(short type) => type is AdWChar or AdVarWChar ? 2 : 1;

        // --- low-level helpers ------------------------------------------------------------------

        private void Fill(int count) => FillInto(_scratch, count);

        private void FillInto(byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = _buffered.Read(buffer, offset, count - offset);
                if (read <= 0) throw new EndOfStreamException("Unexpected end of ADTG row data.");
                offset += read;
            }
        }

        private static void ReadExactly(Stream stream, byte[] buffer, int count)
        {
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read <= 0) throw new EndOfStreamException("Unexpected end of ADTG header.");
                offset += read;
            }
        }

        private static int ReadU16(byte[] b, int offset) => b[offset] | (b[offset + 1] << 8);

        private static uint ReadU32(byte[] b, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(offset, 4));

        private static int IndexOf(byte[] haystack, byte[] needle, int start)
        {
            if (start < 0) start = 0;
            int last = haystack.Length - needle.Length;
            for (int i = start; i <= last; i++)
            {
                int k = 0;
                while (k < needle.Length && haystack[i + k] == needle[k]) k++;
                if (k == needle.Length) return i;
            }
            return -1;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_ownsStream)
            {
                _buffered.Dispose(); // disposes the underlying file stream too
            }
        }
    }
}
