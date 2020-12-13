using RouteDev.Data;

namespace RouteDev.Utils
{
    public static class Constants
    {
        public const byte StorageX = 17;
        public const byte StorageY = 14;
        public static readonly Shop Storage = new Shop(0, StorageX, StorageY);
    }
}