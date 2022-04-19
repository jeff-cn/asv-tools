using System;

namespace Asv.Tools
{
    public class SpanPacketUnsignedIntegerType : ISizedSpanSerializable
    {
        public uint Value { get; set; }

        public SpanPacketUnsignedIntegerType()
        {
            
        }

        public SpanPacketUnsignedIntegerType(uint value)
        {
            Value = value;
        }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Value = BinSerialize.ReadPackedUnsignedInteger(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WritePackedUnsignedInteger(ref buffer, Value);
        }

        public int GetByteSize() => BinSerialize.GetSizeForPackedUnsignedInteger(Value);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
