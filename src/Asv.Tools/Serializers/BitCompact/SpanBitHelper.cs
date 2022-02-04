using System;

namespace Asv.Tools
{
    public  static partial class SpanBitHelper
    {
        
        




        public static uint GetBitU(ref ReadOnlySpan<byte> buff,ref uint pos, uint len)
        {
            uint bits = 0;
            uint i;
            for (i = pos; i < pos + len; i++)
                bits = (uint)((bits << 1) + ((buff[(int)(i / 8)] >> (int)(7 - i % 8)) & 1u));
            pos += len;
            return bits;
        }

        public static uint GetBitUReverse(ref ReadOnlySpan<byte> buff, ref uint pos, uint len)
        {
            uint bits = 0;
            for (var i = (int)(pos + len) - 1; i >= pos; i--)
                bits = (uint)((bits << 1) + ((buff[i / 8] >> 7 - i % 8) & 1u));
            pos += len;
            return bits;
        }

        public static void SetBitU(ref Span<byte> buff, ref uint pos, uint len, uint data)
        {
            var mask = 1u << (int)(len - 1);

            if (len <= 0 || 32 < len) return;

            for (var i = pos; i < pos + len; i++, mask >>= 1)
            {
                if ((data & mask) > 0)
                    buff[(int)(i / 8)] |= (byte)(1u << (int)(7 - i % 8));
                else
                    buff[(int)(i / 8)] &= (byte)(~(1u << (int)(7 - i % 8)));
            }
            pos += len;
        }

        public static void SetBitUReverse(ref Span<byte> buff, ref uint pos, uint len, uint data)
        {
            var mask = 1u;

            if (len <= 0 || 32 < len) return;

            for (var i = pos; i < pos + len; i++, mask <<= 1)
            {
                if ((data & mask) > 0)
                    buff[(int)(i / 8)] |= (byte)(1u << (int)(7 - i % 8));
                else
                    buff[(int)(i / 8)] &= (byte)(~(1u << (int)(7 - i % 8)));
            }
            pos += len;
        }

        public static void SetBitU(ref Span<byte> buff,ref uint pos, uint len, double data)
        {
            SetBitU(ref buff,ref pos, len, (uint)data);
        }

        public static void SetBitUReverse(ref Span<byte> buff, ref uint pos, uint len, double data)
        {
            SetBitUReverse(ref buff, ref pos, len, (uint)data);
        }

        public static int GetBitS(ref ReadOnlySpan<byte> buff, ref uint pos, uint len)
        {
            var bits = GetBitU(ref buff,ref pos, len);
            if (len <= 0 || 32 <= len || (bits & (1u << (int)(len - 1))) == 0)
                return (int)bits;
            return (int)(bits | (~0u << (int)len)); /* extend sign */
        }

        public static int GetBitSReverse(ref ReadOnlySpan<byte> buff, ref uint pos, uint len)
        {
            var bits = GetBitUReverse(ref buff,ref  pos, len);
            if (len <= 0 || 32 <= len || (bits & (1u << (int)(len - 1))) == 0)
                return (int)bits;
            return (int)(bits | (~0u << (int)len)); /* extend sign */
        }

        public static void SetBitS(ref Span<byte> buff,ref uint pos, uint len, int data)
        {
            if (data < 0)
                data |= 1 << (int)(len - 1);
            else
                data &= ~(1 << (int)(len - 1)); /* set sign bit */
            SetBitU(ref buff, ref pos, len, (uint)data);
        }

        public static void SetBitSReverse(ref Span<byte> buff,ref uint pos, uint len, int data)
        {
            if (data < 0)
                data |= 1 << (int)(len - 1);
            else
                data &= ~(1 << (int)(len - 1)); /* set sign bit */
            SetBitUReverse(ref buff, ref pos, len, (uint)data);
        }

        public static void SetBitS(ref Span<byte> buff,ref uint pos, uint len, double data)
        {
            SetBitS(ref buff, ref pos, len, (int)data);
        }

        public static void SetBitSReverse(ref Span<byte> buff,ref uint pos, uint len, double data)
        {
            SetBitSReverse(ref buff, ref pos, len, (int)data);
        }

    }
}
