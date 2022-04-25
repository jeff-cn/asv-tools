using System;

namespace Asv.Tools
{
    public class SessionRecordMetadata: ISizedSpanSerializable
    {
        public const int MetadataFileOffset = 256;

        public SessionFieldSettings Settings { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Settings ??= new SessionFieldSettings();
            Settings.Deserialize(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            Settings.Serialize(ref buffer);
        }

        public int GetByteSize()
        {
            return Settings.GetByteSize();
        }
    }
}
