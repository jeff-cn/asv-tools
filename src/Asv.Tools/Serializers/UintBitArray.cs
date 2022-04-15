using System;
using System.Collections.Generic;

namespace Asv.Tools
{
    public class UintBitArray
    {
        public int Size { get; }

        private static readonly uint[] Bitmask = {
            0x00000001, 0x00000002, 0x00000004, 0x00000008, 0x00000010, 0x00000020, 0x00000040, 0x00000080,
            0x00000100, 0x00000200, 0x00000400, 0x00000800, 0x00001000, 0x00002000, 0x00004000, 0x00008000,
            0x00010000, 0x00020000, 0x00040000, 0x00080000, 0x00100000, 0x00200000, 0x00400000, 0x00800000,
            0x01000000, 0x02000000, 0x04000000, 0x08000000, 0x10000000, 0x20000000, 0x40000000, 0x80000000
        };

        private static readonly uint[] BitmaskXor = {
            0xFFFFFFFE, 0xFFFFFFFD, 0xFFFFFFFB, 0xFFFFFFF7, 0xFFFFFFEF, 0xFFFFFFDF, 0xFFFFFFBF, 0xFFFFFF7F,
            0xFFFFFEFF, 0xFFFFFDFF, 0xFFFFFBFF, 0xFFFFF7FF, 0xFFFFEFFF, 0xFFFFDFFF, 0xFFFFBFFF, 0xFFFF7FFF,
            0xFFFEFFFF, 0xFFFDFFFF, 0xFFFBFFFF, 0xFFF7FFFF, 0xFFEFFFFF, 0xFFDFFFFF, 0xFFBFFFFF, 0xFF7FFFFF,
            0xFEFFFFFF, 0xFDFFFFFF, 0xFBFFFFFF, 0xF7FFFFFF, 0xEFFFFFFF, 0xDFFFFFFF, 0xBFFFFFFF, 0x7FFFFFFF
        };

        private uint _value;

        public UintBitArray(uint value, int size)
        {
            Value = value;
            Size = size;
            if (size > 32)
                throw new ArgumentOutOfRangeException(nameof(size), "Size must be less then 32 bit");
        }

        public UintBitArray(IEnumerable<bool> toArray)
        {
            Value = 0;
            var index = 0;
            foreach (var b in toArray)
            {
                SetBitOpt(ref _value, index++, b);
            }
            Size = index;
            
            if (Size > 32)
                throw new ArgumentOutOfRangeException(nameof(Size), "Size must be less then 32 bit");
        }

        public uint Value
        {
            get => _value;
            set => _value = value;
        }


        public bool this[int index]
        {
            get
            {
                if (index > Size)
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index '{index}' is more then size '{Size}'");
                return GetBitOpt(Value, index);
            }
            set
            {
                if (index > Size)
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index '{index}' is more then size '{Size}'");
                SetBitOpt(ref _value, index, value);
            }
        }

        public static bool GetBitOpt(uint bitfield, int bitIndex)
        {
            return (bitfield & Bitmask[bitIndex]) != 0;
        }

        public static void SetBitOpt(ref uint bitfield, int bitIndex, bool value)
        {
            if (value)
                bitfield |= Bitmask[bitIndex];
            else
                bitfield &= BitmaskXor[bitIndex];
        }


    }
}
