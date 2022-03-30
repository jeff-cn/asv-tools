using System;

namespace Asv.Tools
{
    public class SpanStringType: ISizedSpanSerializable
    {
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

    }
}
