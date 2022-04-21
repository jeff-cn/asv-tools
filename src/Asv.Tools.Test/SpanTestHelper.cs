using System;
using DeepEqual.Syntax;

namespace Asv.Tools.Test
{
    public class SpanTestHelper
    {

        public static void SerializeDeserializeTestBegin(Action<string> output = null)
        {
            output?.Invoke($"{"#",-4} | {"NAME",-25} | {"VALUE",-50} | {"SIZE(bytes)",-3} | COMMENT ");
            output?.Invoke($"----------------------------------------------------------------------------------------------------------------");
        }

        public static bool TestType<T>(T type, Action<string> output = null, string comment = null)
            where T : ISizedSpanSerializable, new()
        {
            var arr = new byte[type.GetByteSize()];
            var span = new Span<byte>(arr);
            type.Serialize(ref span);
            var compare = new T();
            var readSpan = new ReadOnlySpan<byte>(arr, 0, type.GetByteSize());
            compare.Deserialize(ref readSpan);
            var result = type.WithDeepEqual(compare).Compare();
            output?.Invoke(
                $"{(result ? "OK" : "ERR"),-4} | {typeof(T).Name,-25} | {type,-50} | {type.GetByteSize(),-3} | {comment??string.Empty}");
            return result;
        }

    }
}
