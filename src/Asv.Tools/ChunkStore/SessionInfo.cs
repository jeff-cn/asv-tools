using System;

namespace Asv.Tools
{
    public class SessionInfo: SessionSettings
    {
        public Guid Id { get; set; }

        public override void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            base.Deserialize(ref buffer);
            Id = new Guid(BinSerialize.ReadBlock(ref buffer, 16));
        }

        public override void Serialize(ref Span<byte> buffer)
        {
            base.Serialize(ref buffer);
            BinSerialize.WriteBlock(ref buffer,new ReadOnlySpan<byte>(Id.ToByteArray()));
        }

        public override int GetByteSize()
        {
            return base.GetByteSize() + 16 /* size of GUID */;
        }
    }
}
