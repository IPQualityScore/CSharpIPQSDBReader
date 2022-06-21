using System.Net;

namespace IPQSDBReader
{

    public class Utility
    {
        internal static string COUNTRY_LIST_RAW_URL = "https://ipqualityscore.com/api/raw/country/list";
        internal static string COUNTRY_LIST_CACHE_PATH = "countrylist.raw";
        public static bool UpdateCountryList()
        {
            try
            {
                StreamWriter writer = new StreamWriter(COUNTRY_LIST_CACHE_PATH, true);

                String[] raw = new WebClient().DownloadString(COUNTRY_LIST_RAW_URL).Split("\n");
                for (var i = 0; i < raw.Length; i++)
                {
                    writer.Write(raw[i] + "\n");
                }
                writer.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal static int MAX_COUNTRY_CACHE_AGE = 7;
        public static Dictionary<string, string> GetCountryList()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            StreamReader cache;
            try
            {
                long milliseconds = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (!(File.Exists(COUNTRY_LIST_CACHE_PATH)) && !(UpdateCountryList()) || File.GetLastWriteTime(COUNTRY_LIST_CACHE_PATH).Millisecond < (milliseconds - (MAX_COUNTRY_CACHE_AGE * 24 * 60 * 60 * 1000)))
                {
                    throw new IOException("Unable to read/write to countrylist.raw. To do country conversions this file must be available.");
                }
                cache = new StreamReader(COUNTRY_LIST_CACHE_PATH);
            }
            catch (IOException)
            {
                if (!UpdateCountryList())
                {
                    throw new IOException("Unable to read/write to countrylist.raw. To do country conversions this file must be available.");
                }

                cache = new StreamReader(COUNTRY_LIST_CACHE_PATH);
            }

            string line;
            while (!string.ReferenceEquals((line = cache.ReadLine()), null))
            {
                int p = line.IndexOf(':');
                if (p >= 0)
                {
                    string key = line.Substring(0, p - 1);
                    string value = line.Substring(p + 2);
                    result[key] = value;
                }
            }

            if (result.Count == 0)
            {
                throw new IOException("Unable to read/write to countrylist.raw. To do country conversions this file must be available.");
            }

            return result;
        }

        public static byte[] copyOfRange(byte[] src, uint start, uint end)
        {
            uint len = end - start;
            byte[] dest = new byte[len];
            for (int i = 0; i < len; i++)
            {
                dest[i] = src[start + i];
            }

            byte[] result = dest;
            return result;
        }

        public static string SBArrayToString(byte[] data)
        {
            string val = "";
            foreach (byte b in data)
            {
                char c = Convert.ToChar(b);
                if (c == '\0')
                {
                    continue;
                }
                val = (val + Convert.ToChar(b)).Trim();
            }

            return val;
        }

        public static uint uVarInt(byte[] rawData)
        {
            ulong x = 0;
            int s = 0;

            for (int i = 0; i < rawData.Length; i++)
            {
                byte b = rawData[i];
                if (b < 0x80)
                {
                    return (uint)(x | (ulong)(b << s));
                }

                x |= (ulong)(b & 0x7f) << s;
                s += 7;
            }

            return 0;
        }

        public static uint toUnsignedInt(byte[] rawData)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(rawData);

            uint i = BitConverter.ToUInt32(rawData);

            return i;
        }

        public static uint toUnsignedInt(byte x)
        {
            uint i = Convert.ToUInt32(x);
            return i;
        }
    }
}