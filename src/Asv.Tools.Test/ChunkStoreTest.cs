using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeepEqual.Syntax;
using Newtonsoft.Json;
using Xunit;

namespace Asv.Tools.Test
{
    public class ChunkStoreTest
    {
        [Fact]
        public void ArgumentsValidation()
        {

        }

        [Fact]
        public void ReadWriteTest()
        {
            var rootFolder = Path.Combine(Path.GetTempPath(),Path.GetRandomFileName());
            var svc = new ChunkFileStore(rootFolder);
            var metadata = svc.Start(new SessionSettings("rec1", "tag1", "tag2"), new[]
            {
                new SessionFieldSettings(0,"BlaBla",256)
            });

            for (uint i = 0; i < 1000; i++)
            {
                var value = i;
                svc.Append(0, (ref Span<byte> data) =>
                {
                    BinSerialize.WritePackedUnsignedInteger(ref data,value);
                });
            };
            svc.Stop();

            var session = svc.GetSessions();
            Assert.NotNull(session);
            Assert.True(session.Any());
            Assert.Equal(session.First(),metadata.Id);

            var readedMetadata = svc.GetSessionInfo(metadata.Id);
            readedMetadata.WithDeepEqual(metadata).Assert();

            for (uint i = 0; i < 1000; i++)
            {
                uint value = 0;
                svc.ReadRecord(metadata.Id,0,i, (ref ReadOnlySpan<byte> data) =>
                {
                    value = BinSerialize.ReadPackedUnsignedInteger(ref data);
                });
                Assert.Equal(value,i);
            }


        }

       
    }
}
