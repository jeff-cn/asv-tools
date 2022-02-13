namespace Asv.Tools
{
    public interface ISizedSpanSerializable: ISpanSerializable
    {
        uint GetByteSize();
    }
}
