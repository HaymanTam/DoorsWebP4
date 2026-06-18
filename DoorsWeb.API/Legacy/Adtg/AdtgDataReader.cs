using System.Collections;
using System.Data.Common;

namespace DoorsWeb.API.Legacy.Adtg
{
    /// <summary>
    /// Adapts an <see cref="AdtgRecordsetReader"/> to <see cref="DbDataReader"/> so the parsed rows
    /// can be streamed straight into SQL Server via <c>SqlBulkCopy</c> without ever materializing the
    /// whole table in memory (important for the multi-million-row T_Events table).
    /// </summary>
    public sealed class AdtgDataReader : DbDataReader
    {
        private readonly AdtgRecordsetReader _reader;
        private readonly IEnumerator<object?[]> _rows;
        private object?[] _current = Array.Empty<object?>();
        private bool _closed;

        public AdtgDataReader(AdtgRecordsetReader reader)
        {
            _reader = reader;
            _rows = reader.ReadRows().GetEnumerator();
        }

        public override int FieldCount => _reader.Columns.Count;
        public override bool HasRows => true;
        public override bool IsClosed => _closed;
        public override int Depth => 0;
        public override int RecordsAffected => -1;

        public override bool Read()
        {
            if (_rows.MoveNext())
            {
                _current = _rows.Current;
                return true;
            }
            return false;
        }

        public override bool NextResult() => false;

        public override string GetName(int ordinal) => _reader.Columns[ordinal].Name;

        public override int GetOrdinal(string name)
        {
            var columns = _reader.Columns;
            for (int i = 0; i < columns.Count; i++)
            {
                if (string.Equals(columns[i].Name, name, StringComparison.OrdinalIgnoreCase)) return i;
            }
            throw new IndexOutOfRangeException(name);
        }

        public override Type GetFieldType(int ordinal) => AdtgRecordsetReader.ClrTypeOf(_reader.Columns[ordinal]);

        public override string GetDataTypeName(int ordinal) => GetFieldType(ordinal).Name;

        public override object GetValue(int ordinal) => _current[ordinal] ?? DBNull.Value;

        public override int GetValues(object[] values)
        {
            int count = Math.Min(values.Length, FieldCount);
            for (int i = 0; i < count; i++) values[i] = GetValue(i);
            return count;
        }

        public override bool IsDBNull(int ordinal) => _current[ordinal] is null;

        public override object this[int ordinal] => GetValue(ordinal);
        public override object this[string name] => GetValue(GetOrdinal(name));

        public override bool GetBoolean(int ordinal) => (bool)GetValue(ordinal);
        public override byte GetByte(int ordinal) => (byte)GetValue(ordinal);
        public override char GetChar(int ordinal) => (char)GetValue(ordinal);
        public override DateTime GetDateTime(int ordinal) => (DateTime)GetValue(ordinal);
        public override decimal GetDecimal(int ordinal) => (decimal)GetValue(ordinal);
        public override double GetDouble(int ordinal) => Convert.ToDouble(GetValue(ordinal));
        public override float GetFloat(int ordinal) => (float)GetValue(ordinal);
        public override Guid GetGuid(int ordinal) => (Guid)GetValue(ordinal);
        public override short GetInt16(int ordinal) => (short)GetValue(ordinal);
        public override int GetInt32(int ordinal) => (int)GetValue(ordinal);
        public override long GetInt64(int ordinal) => Convert.ToInt64(GetValue(ordinal));
        public override string GetString(int ordinal) => (string)GetValue(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
            => throw new NotSupportedException();

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
            => throw new NotSupportedException();

        public override IEnumerator GetEnumerator() => throw new NotSupportedException();

        public override void Close() => _closed = true;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rows.Dispose();
                _reader.Dispose();
            }
            _closed = true;
            base.Dispose(disposing);
        }
    }
}
