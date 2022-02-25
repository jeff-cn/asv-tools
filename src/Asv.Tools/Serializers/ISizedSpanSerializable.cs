namespace Asv.Tools
{
    public interface ISizedSpanSerializable: ISpanSerializable
    {
        int GetByteSize();
    }
}
