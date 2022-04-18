using System;

namespace Asv.Tools
{
    public class SessionRecordMetadata: SessionRecordSettings, ISizedSpanSerializable
    {
        public const int MetadataFileOffset = 256;

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            base.Deserialize(ref buffer);
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            base.Serialize(ref buffer);
        }

        public override int GetByteSize()
        {
            return base.GetByteSize();
        }
    }
}
