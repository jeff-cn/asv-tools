using System;

namespace Asv.Tools
{
    public interface ISpanBitCompactSerializable:ISpanSerializable
    {
        uint GetBitSize();
        void Deserialize(ref ReadOnlySpan<byte> buffer, ref uint bitPosition);
        void Serialize(ref Span<byte> buffer, ref uint bitPosition);
    }
}
