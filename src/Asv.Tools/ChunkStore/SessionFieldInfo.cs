using System;

namespace Asv.Tools
{
    public class SessionFieldInfo:ISizedSpanSerializable
    {
        public SessionRecordMetadata Metadata { get; set; }
        public uint SizeInBytes { get; set; }
        public uint Count { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Metadata ??= new SessionRecordMetadata();
            Metadata.Deserialize(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            Metadata.Serialize(ref buffer);
        }

        public int GetByteSize()
        {
            return Metadata.GetByteSize();
        }
    }
}
