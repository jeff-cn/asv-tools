using System;

namespace Asv.Tools
{
    public class SpanByteType : ISizedSpanSerializable
    {
        public SpanByteType()
        {
            
        }

        public SpanByteType(byte value)
        {
            Value = value;
        }

        public byte Value { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Value = BinSerialize.ReadByte(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteByte(ref buffer, Value);
        }

        public int GetByteSize() => sizeof(byte);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
