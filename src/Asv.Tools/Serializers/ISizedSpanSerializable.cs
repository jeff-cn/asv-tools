using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Asv.Tools
{
    public interface ISizedSpanSerializable: ISpanSerializable
    {
        int GetByteSize();
    }


    public delegate T DeserializeDelegate<out T>(ref ReadOnlySpan<byte> data);
    public delegate void SerializeDelegate<in T>(ref Span<byte> data, T value);
    public delegate int SerializeSizeDelegate<in T>(T value);

    public static class SpanSerializableHelper
    {
        public static T Deserialize<T>(ref ReadOnlySpan<byte> data) where T : ISizedSpanSerializable, new()
        {
            var result = new T();
            result.Deserialize(ref data);
            return result;
        }
        public static void Serialize<T>(ref Span<byte> data, T value) where T : ISizedSpanSerializable, new()
        {
            value.Serialize(ref data);
        }

        public static int SerializeSize<T>(T value) where T : ISizedSpanSerializable, new()
        {
            return value.GetByteSize();
        }

        public static uint CalculateCrc32QHash(this IEnumerable<ISizedSpanSerializable> types, uint initValue = 0U)
        {
            return types.Aggregate(initValue, (current, desc) => CalculateCrc32QHash(desc, current));
        }

        public static uint CalculateCrc32QHash(this ISizedSpanSerializable type, uint initValue = 0U)
        {
            var data = ArrayPool<byte>.Shared.Rent(type.GetByteSize());
            try
            {
                var span = new Span<byte>(data, 0, type.GetByteSize());
                var size = span.Length;
                type.Serialize(ref span);
                return Crc32Q.Calc(data, size, initValue);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(data);
            }
        }

        public static void WriteToStream(this ISpanSerializable item, Stream file, int itemMaxSize)
        {
            var array = ArrayPool<byte>.Shared.Rent(itemMaxSize);
            var span = new Span<byte>(array, 0, itemMaxSize);
            item.Serialize(ref span);
            for (var i = 0; i < span.Length; i++) span[i] = 0;
            file.Write(array, 0, itemMaxSize);
        }

        public static void ReadFromStream(this ISpanSerializable item, Stream file, int offset)
        {
            var array = ArrayPool<byte>.Shared.Rent(offset);
            var readed = file.Read(array, 0, offset);
            if (readed != offset)
                throw new Exception(
                    $"Error to read item {item}: file length error. Want read {offset} bytes. Got {readed} bytes.");
            var span = new ReadOnlySpan<byte>(array, 0, offset);
            item.Deserialize(ref span);
        }

    }
}
