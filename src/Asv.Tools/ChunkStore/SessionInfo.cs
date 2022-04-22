using System;

namespace Asv.Tools
{
    public class SessionInfo:ISizedSpanSerializable
    {
        public SessionMetadata Metadata { get; set; }
        public uint RecordsCount { get; set; }

        public void Deserialize(ref ReadOnlySpan<byte> buffer)
        {
            Metadata.Deserialize(ref buffer);
            RecordsCount = BinSerialize.ReadPackedUnsignedInteger(ref buffer);
        }

        public void Serialize(ref Span<byte> buffer)
        {
            Metadata.Serialize(ref buffer);
            BinSerialize.WritePackedUnsignedInteger(ref buffer, RecordsCount);
        }

        public int GetByteSize()
        {
            return Metadata.GetByteSize() + BinSerialize.GetSizeForPackedUnsignedInteger(RecordsCount);
        }
    }
}
