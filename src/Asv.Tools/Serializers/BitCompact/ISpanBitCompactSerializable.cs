using System;

namespace Asv.Tools
{
    public interface ISpanBitCompactSerializable:ISizedSpanSerializable
    {
        uint GetBitSize();
        void Deserialize(ref ReadOnlySpan<byte> buffer, ref uint bitPosition);
        void Serialize(ref Span<byte> buffer, ref uint bitPosition);
    }
}
