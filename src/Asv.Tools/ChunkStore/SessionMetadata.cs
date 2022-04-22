using System;

namespace Asv.Tools
{
    public class SessionMetadata : ISizedSpanSerializable
    {
        public Guid Id { get; set; }
        public SessionSettings Settings { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Settings.Deserialize(ref buffer);
            Id = new Guid(BinSerialize.ReadBlock(ref buffer, 16));
        }

        public void Serialize(ref Span<byte> buffer)
        {
            Settings.Serialize(ref buffer);
            BinSerialize.WriteBlock(ref buffer, new ReadOnlySpan<byte>(Id.ToByteArray()));
        }

        public int GetByteSize()
        {
            return Settings.GetByteSize() + 16 /* size of GUID */;
        }
    }
}
