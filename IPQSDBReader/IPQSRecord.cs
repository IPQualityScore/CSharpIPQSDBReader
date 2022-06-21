using System.Text;

namespace IPQSDBReader
{
    public class IPQSRecord
    {
        private FileReader fileReader;
        public bool parse(FileReader file, byte[] raw)
        {
            this.fileReader = file;

            uint current_byte = 0;
            if (file.BinaryData)
            {
                processFirstByte(raw[0]);
                processSecondByte(raw[1]);

                ConnectionType = ConnectionType.create(raw[2]);
                AbuseVelocity = AbuseVelocity.create(raw[2]);
                current_byte = 3;
            }
            else
            {
                ConnectionType = ConnectionType.create(raw[0]);
                AbuseVelocity = AbuseVelocity.create(raw[0]);
                current_byte = 1;
            }

            FraudScore = (new FraudScore());
            for (int i = 0; i < file.Columns.Count; i++)
            {
                Column column = file.Columns[i];
                if (column == null)
                {
                    throw new IOException("Invalid or nonexistant IP address specified for lookup. (EID: 12)");
                }

                Column c = new Column();
                c.Name = column.Name;
                c.Type = column.Type;
                
                switch (column.Name)
                {
                    case "ASN":
                        ASN = (int)Utility.toUnsignedInt(Utility.copyOfRange(raw, current_byte, current_byte + 4));
                        c.RawValue = ASN.ToString();

                        current_byte += 4;
                        break;
                    case "Latitude":
                        Latitude = BitConverter.ToSingle(BitConverter.GetBytes(Utility.toUnsignedInt(Utility.copyOfRange(raw, current_byte, current_byte + 4))));
                        c.RawValue = Latitude.ToString();

                        current_byte += 4;
                        break;
                    case "Longitude":
                        Longitude = BitConverter.ToSingle(BitConverter.GetBytes(Utility.toUnsignedInt(Utility.copyOfRange(raw, current_byte, current_byte + 4))));
                        c.RawValue = Latitude.ToString();

                        current_byte += 4;
                        break;
                    case "ZeroFraudScore":
                        FraudScore.setFraudScore((int)Utility.uVarInt(Utility.copyOfRange(raw, current_byte, current_byte + 1)), 0);
                        c.RawValue = (FraudScore.forStrictness(0)).ToString();

                        current_byte++;
                        break;
                    case "OneFraudScore":
                        FraudScore.setFraudScore((int)Utility.uVarInt(Utility.copyOfRange(raw, current_byte, current_byte + 1)), 1);
                        c.RawValue = (FraudScore.forStrictness(1)).ToString();

                        current_byte++;
                        break;
                    default:
                        if ((c.Type != null) && c.Type.Has(Bitmask.StringData))
                        {
                            c.RawValue = getRangedStringValue(this.fileReader, Utility.copyOfRange(raw, current_byte, current_byte + 4));
                            current_byte += 4;
                        }
                        break;
                }

                switch (c.Name)
                {
                    case "Country":
                        Country = c.RawValue;
                        break;
                    case "City":
                        City = c.RawValue;
                        break;
                    case "Region":
                        Region = c.RawValue;
                        break;
                    case "ISP":
                        ISP = c.RawValue;
                        break;
                    case "Organization":
                        Organization = c.RawValue;
                        break;
                    case "Timezone":
                        Timezone = c.RawValue;
                        break;
                }

                Columns.Add(c);
            }

            return true;
        }

        public string getRangedStringValue(FileReader file, byte[] pointer)
        {
            file.Handler.Seek(BitConverter.ToUInt32(pointer), SeekOrigin.Begin);
            int totalbvtes = file.Handler.ReadByte();

            byte[] raw = new byte[totalbvtes];
            file.Handler.Read(raw);

            return Encoding.UTF8.GetString(raw);
        }

        private void processFirstByte(byte b)
        {
            Bitmask mask = Bitmask.Create(b);
            if (mask.Has(Bitmask.IsProxy))
            {
                Proxy = true;
            }

            if (mask.Has(Bitmask.IsVPN))
            {
                VPN = true;
            }

            if (mask.Has(Bitmask.IsTOR))
            {
                TOR = true;
            }

            if (mask.Has(Bitmask.IsCrawler))
            {
                Crawler = true;
            }

            if (mask.Has(Bitmask.IsBot))
            {
                Bot = true;
            }

            if (mask.Has(Bitmask.RecentAbuse))
            {
                RecentAbuse = true;
            }

            if (mask.Has(Bitmask.IsBlackList))
            {
                Blacklisted = true;
            }

            if (mask.Has(Bitmask.IsPrivate))
            {
                Private = true;
            }
        }

        private void processSecondByte(byte b)
        {
            Bitmask mask = Bitmask.Create(b);
            if (mask.Has(Bitmask.IsMobile))
            {
                Mobile = true;
            }

            if (mask.Has(Bitmask.HasOpenPorts))
            {
                HasOpenPorts = true;
            }

            if (mask.Has(Bitmask.IsHostingProvider))
            {
                HostingProvider = true;
            }

            if (mask.Has(Bitmask.ActiveVPN))
            {
                ActiveVPN = true;
            }

            if (mask.Has(Bitmask.ActiveTOR))
            {
                ActiveTOR = true;
            }

            if (mask.Has(Bitmask.PublicAccessPoint))
            {
                PublicAccessPoint = true;
            }
        }

        public bool Proxy;
        public bool VPN;
        public bool TOR;
        public bool Crawler;
        public bool Bot;
        public bool RecentAbuse;
        public bool Blacklisted;
        public bool Private;
        public bool Mobile;
        public bool HasOpenPorts;
        public bool HostingProvider;
        public bool ActiveVPN;
        public bool ActiveTOR;
        public bool PublicAccessPoint;
        public ConnectionType ConnectionType;
        public AbuseVelocity AbuseVelocity;
        public string Country;
        public string City;
        public string Region;
        public string ISP;
        public string Organization;
        public string Timezone;
        public int ASN;
        public float Latitude;
        public float Longitude;
        public FraudScore FraudScore;
        public List<Column> Columns = new List<Column>();

        public bool isProxy() { return Proxy; }
        public bool isVPN() { return VPN; }
        public bool isTOR() { return TOR; }
        public bool isCrawler() { return Crawler; }
        public bool isBot() { return Bot; }
        public bool hasRecentAbuse() { return RecentAbuse; }
        public bool isBlacklisted() { return Blacklisted; }
        public bool isPrivate() { return Private; }
        public bool isMobile() { return Mobile; }
        public bool hasOpenPorts() { return HasOpenPorts; }
        public bool isHostingProvider() { return HostingProvider; }
        public bool isActiveVPN() { return ActiveVPN; }
        public bool isActiveTOR() { return ActiveTOR; }
        public bool isPublicAccessPoint() { return PublicAccessPoint; }
        public ConnectionType getConnectionType() { return ConnectionType; }
        public AbuseVelocity getAbuseVelocity() { return AbuseVelocity; }
        public string getCountry() { return Country; }
        public string getCity() { return City; }
        public string getRegion() { return Region; }
        public string getISP() { return ISP; }
        public string getOrganization() { return Organization; }
        public string getTimezone() { return Timezone; }
        public int getASN() { return ASN; }
        public float getLatitude() { return Latitude; }
        public float getLongitude() { return Longitude; }
        public FraudScore getFraudScore() { return FraudScore; }
    }
}