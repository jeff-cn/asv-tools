using System;

namespace Asv.Tools
{
    public interface ISpanBitCompactSerializable:ISizedSpanSerializable
    {
        int GetBitSize();
        void Deserialize(ReadOnlySpan<byte> buffer, ref int bitPosition);
        void Serialize(Span<byte> buffer, ref int bitPosition);
    }
}
