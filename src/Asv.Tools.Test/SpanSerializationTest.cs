using System;
using System.Collections.Generic;
using System.Linq;
using DeepEqual.Syntax;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Tools.Test
{
    public class SpanSerializationTest
    {
        private readonly ITestOutputHelper _output;

        public SpanSerializationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        

        [Fact]
        public void StandartTypes()
        {
            SpanTestHelper.SerializeDeserializeTestBegin(_output.WriteLine);
            var data = new byte[256];
            new Random().NextBytes(data);
            var testResult = new List<bool>
            {
                SpanTestHelper.TestType(new SpanVoidType(), _output.WriteLine),
                SpanTestHelper.TestType(new SpanBoolType(true), _output.WriteLine),
                SpanTestHelper.TestType(new SpanByteArrayType(data), _output.WriteLine),
                SpanTestHelper.TestType(new SpanByteType(byte.MaxValue), _output.WriteLine),
                SpanTestHelper.TestType(new SpanByteType(byte.MinValue), _output.WriteLine),
                SpanTestHelper.TestType(new SpanDoubleByteType(byte.MinValue, byte.MaxValue), _output.WriteLine),
                SpanTestHelper.TestType(new SpanPacketUnsignedIntegerType(uint.MaxValue), _output.WriteLine),
                SpanTestHelper.TestType(new SpanPacketIntegerType(int.MaxValue), _output.WriteLine),
                SpanTestHelper.TestType(new SpanStringType("asdasd ASDSAD 984984"), _output.WriteLine)
            };


            Assert.True(testResult.All(_=>_));
        }
    }
}
