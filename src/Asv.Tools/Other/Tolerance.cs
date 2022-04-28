namespace Asv.Tools
{
    public class Tolerance<T> where T : struct
    {
        public T Upper { get; set; }
        public T Lower { get; set; }
    }
}