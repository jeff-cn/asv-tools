using System;

namespace Asv.Tools
{
    public class SessionInfo:ISizedSpanSerializable
    {
        public SessionMetadata Metadata { get; set; }
        public uint FieldsCount { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Metadata ??= new SessionMetadata();
            Metadata.Deserialize(ref buffer);
            FieldsCount = BinSerialize.ReadPackedUnsignedInteger(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            Metadata.Serialize(ref buffer);
            BinSerialize.WritePackedUnsignedInteger(ref buffer, FieldsCount);
        }

        public int GetByteSize()
        {
            return Metadata.GetByteSize() + BinSerialize.GetSizeForPackedUnsignedInteger(FieldsCount);
        }
    }
}
