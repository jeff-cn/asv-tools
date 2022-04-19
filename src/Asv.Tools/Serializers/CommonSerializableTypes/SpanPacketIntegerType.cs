using System;

namespace Asv.Tools
{
    public class SpanPacketIntegerType : ISizedSpanSerializable
    {
        public int Value { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Value = BinSerialize.ReadPackedInteger(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            BinSerialize.WritePackedInteger(ref buffer,Value);
        }

        public int GetByteSize() => BinSerialize.GetSizeForPackedInteger(Value);

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class SpanPacketUnsignedIntegerType : ISizedSpanSerializable
    {
        public uint Value { get; set; }

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
