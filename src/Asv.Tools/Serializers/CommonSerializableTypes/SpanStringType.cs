using System;

namespace Asv.Tools
{
    public class SpanStringType: ISizedSpanSerializable
    {
        public SpanStringType()
        {
            
        }

        public SpanStringType(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Value = BinSerialize.ReadString(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WriteString(ref buffer, Value ?? string.Empty);
        }

        public int GetByteSize() => BinSerialize.GetSizeForString(Value);


        public override string ToString()
        {
            return Value ?? "NULL";
        }
    }
}
