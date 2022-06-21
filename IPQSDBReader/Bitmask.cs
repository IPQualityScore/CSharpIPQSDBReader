
namespace IPQSDBReader
{
    public class Bitmask
    {
        public static readonly int ReaderVersion = 1;

        public static readonly int IPv4Map = 1;
        public static readonly int IPv6Map = 2;
        public static readonly int BlacklistData = 4;
        public static readonly int BinaryData = 128;

        public static readonly int TreeData = 4;
        public static readonly int StringData = 8;
        public static readonly int SmallIntData = 16;
        public static readonly int IntData = 32;
        public static readonly int FloatData = 64;

        public static readonly int IsProxy = 1;
        public static readonly int IsVPN = 2;
        public static readonly int IsTOR = 4;
        public static readonly int IsCrawler = 8;
        public static readonly int IsBot = 16;
        public static readonly int RecentAbuse = 32;
        public static readonly int IsBlackList = 64;
        public static readonly int IsPrivate = 123;

        public static readonly int IsMobile = 1;
        public static readonly int HasOpenPorts = 2;
        public static readonly int IsHostingProvider = 4;
        public static readonly int ActiveVPN = 8;
        public static readonly int ActiveTOR = 16;
        public static readonly int PublicAccessPoint = 32;
        public static readonly int ReservedOne = 64;
        public static readonly int ReservedTwo = 128;

        public static readonly int ReservedThree = 1;
        public static readonly int ReservedFour = 2;
        public static readonly int ReservedFive = 4;
        public static readonly int ConnectionTypeOne = 8;
        public static readonly int ConnectionTypeTwo = 16;
        public static readonly int ConnectionTypeThree = 32;
        public static readonly int AbuseVelocityOne = 64;
        public static readonly int AbuseVelocityTwo = 128;

        public int Raw;


        public static Bitmask Create(byte raw)
        {
            return Create((int)Utility.toUnsignedInt(raw));
        }

        public bool Has(int value)
        {
            return ((this.Raw & value) != 0);
        }

        public void Set(int value)
        {
            this.Raw = this.Raw | value;
        }

        public static Bitmask Create(int raw)
        {
            Bitmask bm = new Bitmask();
            bm.Set(raw);
            return bm;
        }
    }
}