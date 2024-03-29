using System;
using System.Collections.Generic;
using DynamicData;

namespace Asv.Tools
{
    public class SpanByteArrayType : SpanArrayType<byte>
    {
        public SpanByteArrayType()
        {
            
        }

        public SpanByteArrayType(IEnumerable<byte> values)
        {
            Items.AddRange(values);
        }

        protected override void InternalWriteItem(ref Span<byte> buffer, byte item)
        {
            BinSerialize.WriteByte(ref buffer,item);
        }

        protected override byte InternalReadItem(ref ReadOnlySpan<byte> buffer)
        {
            return BinSerialize.ReadByte(ref buffer);
        }

        protected override int InternalGetItemsSize(byte arg)
        {
            return sizeof(byte);
        }

        public override string ToString()
        {
            if (Items == null) return "[null]";
            return $"BYTE[{Items.Count}]";
        }
    }
}
