using System;

namespace Asv.Tools
{
    public interface ISpanSerializable
    {
        void Deserialize(ref ReadOnlySpan<byte> buffer);

        void Serialize(ref Span<byte> buffer);
    }
}
