using System.Text;

namespace Asv.Tools
{
    public class TextReaderBaseConfig
    {
        public int MaxMessageSize = 1024;
        public char StartByte = '\n';
        public char StopByte = '\r';
        public readonly Encoding DefaultEncoding = Encoding.UTF8;
    }
}
