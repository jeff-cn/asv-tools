using System;

namespace Asv.Tools
{
    public delegate T DeserializeDelegate<out T>(ref ReadOnlySpan<byte> data);
    public delegate void SerializeDelegate<in T>(ref Span<byte> data, T value);
    public delegate uint SerializeSizeDelegate<in T>(T value);

    public static class SerializeDelegateHelper
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

        public static uint SerializeSize<T>(T value) where T : ISizedSpanSerializable, new()
        {
            return value.GetByteSize();
        }
    }
}
