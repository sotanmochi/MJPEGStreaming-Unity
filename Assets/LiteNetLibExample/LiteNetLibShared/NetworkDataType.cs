namespace LiteNetLibExample
{
    public enum NetworkDataType
    {
        PlayerTransform,
        PlayerTransformArray,
        PlayerLeave,
    }

    public class NetworkDataSize
    {
        public const int PlayerIdAndTransform = sizeof(int) + sizeof(float) * 7;
        public const int Transform = sizeof(float) * 7;
    }
}
