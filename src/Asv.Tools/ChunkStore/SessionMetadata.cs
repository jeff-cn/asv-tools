using System;

namespace Asv.Tools
{
    public class SessionMetadata: SessionSettings,ISizedSpanSerializable
    {
        public Guid Id { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public void Serialize(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public int GetByteSize()
        {
            throw new NotImplementedException();
        }
    }
}
