
namespace IPQSDBReader
{
    public class CSharpDBReaderExample
    {
        public static void example()
        {
            Test("8.8.0.0", Directory.GetCurrentDirectory() + "\\IPQualityScore-IP-Reputation-Database-IPv4.ipqs");
        }
        public static void Test(string ip, string filePath)
        {
            try
            {
                DBReader Reader = new DBReader();
                FileReader File = Reader.Open(filePath);
                IPQSRecord Record = File.Fetch(ip);

                if (Record.isProxy())
                {
                    Console.WriteLine(ip + " is a proxy.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a proxy.");
                }

                if (Record.isVPN())
                {
                    Console.WriteLine(ip + " is a vpn.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a vpn.");
                }

                if (Record.isTOR())
                {
                    Console.WriteLine(ip + " is a tor node.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a tor node.");
                }

                if (Record.isCrawler())
                {
                    Console.WriteLine(ip + " is a crawler.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a crawler.");
                }

                if (Record.isBot())
                {
                    Console.WriteLine(ip + " is likely a bot.");
                }
                else
                {
                    Console.WriteLine(ip + " is likely not a bot.");
                }

                if (Record.hasRecentAbuse())
                {
                    Console.WriteLine(ip + " has recently engaged in abuse.");
                }
                else
                {
                    Console.WriteLine(ip + " has not engaged in abuse recently.");
                }

                if (Record.isBlacklisted())
                {
                    Console.WriteLine(ip + " is blacklisted.");
                }
                else
                {
                    Console.WriteLine(ip + " is not blacklisted.");
                }

                if (Record.isPrivate())
                {
                    Console.WriteLine(ip + " is a private (non routeable) IP address.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a private IP address.");
                }

                if (Record.isMobile())
                {
                    Console.WriteLine(ip + " is associated with a mobile carrier.");
                }
                else
                {
                    Console.WriteLine(ip + " is not likely to be a mobile carrier.");
                }

                if (Record.hasOpenPorts())
                {
                    Console.WriteLine(ip + " has open ports.");
                }
                else
                {
                    Console.WriteLine(ip + " does not have open ports.");
                }

                if (Record.isHostingProvider())
                {
                    Console.WriteLine(ip + " is a hosting provider.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a hosting provider.");
                }

                if (Record.isActiveVPN())
                {
                    Console.WriteLine(ip + " is an active VPN.");
                }
                else
                {
                    Console.WriteLine(ip + " is not an active VPN.");
                }

                if (Record.isActiveTOR())
                {
                    Console.WriteLine(ip + " is an active TOR node.");
                }
                else
                {
                    Console.WriteLine(ip + " is not an active TOR node.");
                }

                if (Record.isPublicAccessPoint())
                {
                    Console.WriteLine(ip + " is a public access point.");
                }
                else
                {
                    Console.WriteLine(ip + " is not a public access point.");
                }

                Console.WriteLine(ip + " is from " + Record.getCity() + ", " + Record.getCountry() + " (" + Record.Region + ")");
                Console.WriteLine(ip + "'s ISP is " + Record.getISP() + " and is owned by " + Record.getOrganization() + ".");
                Console.WriteLine(ip + " has an ASN of " + Record.getASN() + ".");
                Console.WriteLine(ip + " has a timezone of " + Record.Timezone);
                Console.WriteLine(ip + " has a geographic location of approximately: " + Record.getLatitude() + ", " + Record.getLongitude());
                Console.WriteLine(ip + " is a " + Record.getConnectionType() + " connection. ");
                Console.WriteLine(ip + " has a " + Record.getAbuseVelocity() + " abuse velocity.");
                Console.WriteLine(ip + " has a zipcode of " + Record.getZipcode());
                Console.WriteLine(ip + " has a hostname of " + Record.getHostname());
                Console.WriteLine(ip + " has a fraud score of " + Record.getFraudScore().forStrictness(0) + " for strictness level zero.");
                Console.WriteLine(ip + " has a fraud score of " + Record.getFraudScore().forStrictness(1) + " for strictness level one.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
