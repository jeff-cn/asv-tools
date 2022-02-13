using System;

namespace Asv.Tools
{
    public abstract class SpanBitCompactSerializableBase : ISpanBitCompactSerializable
    {
        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            uint bitIndex = 0;
            Deserialize(ref buffer,ref bitIndex);
            buffer = bitIndex % 8.0 == 0 ? buffer.Slice((int)(bitIndex / 8)) : buffer.Slice((int)(bitIndex / 8) + 1);
        }
    
        public void Serialize(ref Span<byte> buffer)
        {
            uint bitIndex = 0;
            Serialize(ref buffer, ref bitIndex);
            buffer = bitIndex % 8.0 == 0 ? buffer.Slice((int)(bitIndex / 8)) : buffer.Slice((int)(bitIndex / 8) + 1);
        }
    
        public uint GetByteSize()
        {
            var bitSize = GetBitSize();
            var size = (bitSize / 8);
            return bitSize % 8.0 == 0 ? size : size + 1U;
        }
    
        public abstract uint GetBitSize();
        public abstract void Deserialize(ref ReadOnlySpan<byte> buffer, ref uint bitPosition);
        public abstract void Serialize(ref Span<byte> buffer, ref uint bitPosition);
    
    }
}
