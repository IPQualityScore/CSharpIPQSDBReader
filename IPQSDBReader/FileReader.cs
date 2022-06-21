using System.Text;
using System.Net;
using System.Text.RegularExpressions;

namespace IPQSDBReader
{

    public class FileReader
    {

        public int[] Read(int length)
        {
            if (Handler == null)
            {
                return new int[0];
            }

            try
            {
                int[] a = new int[length];

                for (int i = 0; i < length; i++)
                {
                    int b = Handler.ReadByte();
                    a[i] = b;
                }
                return a;
            }
            catch (Exception)
            {
                return new int[0];
            }

        }

        public byte[] Readbytes(int length)
        {
            if (Handler == null)
            {
                return new byte[0];
            }

            try
            {
                byte[] a = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    int b = Handler.ReadByte();
                    a[i] = (byte)b;
                }
                return a;
            }
            catch (Exception)
            {
                return new byte[0];
            }

        }
        public IPQSRecord Fetch(string ip)
        {
            IPQSRecord record = new IPQSRecord();

            if (!isValidIP(ip))
            {
                throw new IOException("Invalid IP address specified for lookup.");
            }

            int position = 0;
            uint[] previous = new uint[128];
            uint file_position = (uint)(TreeStart + 5);
            StringBuilder literal = new StringBuilder(ConvertIPAddressToBinary(ip));

            for (int l = 0; l < 257; l++)
            {
                previous[position] = file_position;
                if (literal.Length <= position)
                {
                    throw new IOException("Invalid or nonexistent IP address specified for lookup. (EID: 8)");
                }

                byte[] read = new byte[4];
                if (literal[position] == '0')
                {
                    Handler.Seek(file_position, SeekOrigin.Begin);
                    Handler.Read(read);
                    file_position = Utility.toUnsignedInt(read);
                }
                else
                {
                    Handler.Seek(file_position + 4, SeekOrigin.Begin);
                    Handler.Read(read);
                    file_position = Utility.toUnsignedInt(read);
                }

                if (this.BlacklistFile == false && file_position == 0)
                {
                    for (int i = 0; i <= position; i++)
                    {
                        if (literal[position - i] == '1')
                        {
                            literal[position - i] = '0';

                            for (int n = (position - i + 1); n < literal.Length; n++)
                            {
                                literal[n] = '1';
                            }

                            position = position - i;
                            file_position = previous[position];
                            break;
                        }
                    }

                    continue;
                }

                if (file_position < TreeEnd)
                {
                    if (file_position == 0)
                    {
                        break;
                    }

                    position++;
                    continue;
                }

                byte[] raw = new byte[RecordBytes];
                Handler.Seek(file_position, SeekOrigin.Begin);
                Handler.Read(raw);

                if (record.parse(this, raw))
                {
                    return record;
                }

                throw new IOException("Invalid or nonexistent IP address specified for lookup. (EID: 22)");
            }
            throw new IOException("Invalid or nonexistent IP address specified for lookup. (EID: 32)");
        }
        static string ConvertIPAddressToBinary(string input)
        {
            string result = "";
            try
            {
                string[] part;
                IPAddress? address;
                if (IPAddress.TryParse(input, address: out address))
                {
                    switch (address.AddressFamily)
                    {
                        case System.Net.Sockets.AddressFamily.InterNetwork:
                            part = input.Split('.');
                            for (int i = 0; i < part.Length; i++)
                            {
                                result += Convert.ToString(Convert.ToUInt16(part[i], 10), 2).PadLeft(8, '0');
                            }
                            break;
                        case System.Net.Sockets.AddressFamily.InterNetworkV6:
                            part = input.Split(':');
                            for (int i = 0; i < part.Length; i++)
                            {
                                result += Convert.ToString(Convert.ToUInt32(part[i], 16), 2).PadLeft(16, '0');
                            }
                            break;
                        default: throw new Exception("Invalid or nonexistant IP address specified for lookup. (EID: 13)");
                    }
                }
            }
            catch
            {
                throw new Exception("Invalid or nonexistant IP address specified for lookup. (EID: 13)");
            }
            return result;
        }

        private const string IPV4_PATTERN = "^(([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\\.(?!$)|$)){4}$";
        private const string IPV6_PATTERN_GENERIC = "([0-9a-f]{1,4}:){7}([0-9a-f]){1,4}";
        private const string IPV6_PATTERN_SHORTENED = "^((?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)::((?:[0-9A-Fa-f]{1,4}(?::[0-9A-Fa-f]{1,4})*)?)$";
        private static readonly Regex ValidIPv4 = new Regex(@IPV4_PATTERN);
        private static readonly Regex ValidIPv6Generic = new Regex(@IPV6_PATTERN_GENERIC);
        private static readonly Regex ValidIPv6Shortened = new Regex(@IPV6_PATTERN_SHORTENED);

        private bool isValidIP(string ip)
        {
            if (IPv6 == true)
            {
                if (ip.Contains("."))
                {
                    throw new IOException("Attempted to look up IPv4 using IPv6 database file. Aborting.");
                }

                if (ValidIPv6Generic.IsMatch(ip) || ValidIPv6Shortened.IsMatch(ip))
                {
                    return true;
                }
            }
            else
            {
                if (ip.Contains(":"))
                {
                    throw new IOException("Attempted to look up IPv6 using IPv4 database file. Aborting.");
                }

                return ValidIPv4.IsMatch(ip);
            }

            return false;
        }

		public FileStream Handler;
		public uint TotalBytes;
		public uint RecordBytes;
		public uint TreeStart;
		public long TreeEnd;
		public bool BlacklistFile;
		public bool IPv6;
		public bool Valid;
		public bool BinaryData;
		public List<Column> Columns = new List<Column>();
        
		private Dictionary<string, string> countrylist = new Dictionary<string, string>();
        public string ConvertCountry(string cc)
        {
            if (countrylist == null)
            {
                countrylist = Utility.GetCountryList();
            }

            return countrylist[cc];
        }
    }
}