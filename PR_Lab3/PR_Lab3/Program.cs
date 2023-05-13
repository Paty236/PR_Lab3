using DnsClient;
using System.Net;

namespace DNSResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a DNS lookup client with Google's public DNS server as the default
            var dnsServer = new LookupClient(IPAddress.Parse("8.8.8.8"));

            // Display available commands to the user
            Console.WriteLine("Available commands:");
            Console.WriteLine("resolve <ip or domain>");
            Console.WriteLine("use dns <ip>");

            // Continuously prompt user for input
            while (true)
            {
                // Display current DNS server being used
                Console.Write($"{dnsServer.NameServers.FirstOrDefault()}> ");

                // Read user input and split into tokens
                var command = Console.ReadLine();
                var tokens = command!.Split(' ');

                // Handle "resolve" command
                if (tokens[0] == "resolve")
                {
                    // Ensure that a domain or IP address is specified
                    if (tokens.Length < 2)
                    {
                        Console.WriteLine("Error: Please specify a domain or IP address.");
                        continue;
                    }
                    else
                    {
                        string query = tokens[1];

                        try
                        {
                            // If query is an IP address, resolve its hostname
                            if (IPAddress.TryParse(query, out _))
                            {
                                string host = dnsServer.GetHostEntry(IPAddress.Parse(query)).HostName;

                                Console.WriteLine(host);
                            }
                            // If query is a domain, resolve its IP addresses
                            else
                            {
                                IPAddress[] addresses = Dns.GetHostAddresses(query);

                                foreach (IPAddress address in addresses)
                                {
                                    Console.WriteLine(address);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error resolving DNS: {e.Message}");
                        }
                    }
                }
                // Handle "use dns" command
                else if (tokens[0] == "use" && tokens[1] == "dns")
                {
                    // Ensure that a DNS server IP address is specified
                    if (tokens.Length < 3)
                    {
                        Console.WriteLine("Invalid command. Usage: use dns <ip>");
                        continue;
                    }
                    else
                    {
                        string newDns = tokens[2];

                        try
                        {
                            // Test if the specified DNS server is valid
                            Dns.GetHostEntry(newDns);
                            dnsServer = new LookupClient(IPAddress.Parse(newDns));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error setting DNS server: {e.Message}");
                        }
                    }
                }
                // Handle unknown command
                else
                {
                    Console.WriteLine("Unknown command.");
                }
            }
        }
    }
}