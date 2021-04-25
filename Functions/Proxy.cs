using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Aldi_Monitor.Models;

namespace Aldi_Monitor.Functions
{
    internal class Proxy
    {
        public static WebProxy GetNewProxy()
        {
            if (Globals.Proxies.Count is 0)
            {
                var lines = File.ReadAllLines($"{Directory.GetCurrentDirectory()}/proxies.txt");

                foreach (var proxy in lines)
                {
                    var split = proxy.Split(':');

                    try
                    {
                        Globals.Proxies?.Add(split.Length == 4
                            ? Parse(split[0], split[1], split[2], split[3])
                            : Parse(split[0], split[1], null, null));

                    }
                    catch
                    {
                        // do nothing
                    }
                }

                if (!Globals.Proxies.Any()) throw new Exception("No proxies in proxy file!");
            }

            if (Globals.ProxyNum > Globals.Proxies.Count - 1) Globals.ProxyNum = 0;

            var randomProxy = Globals.Proxies.ElementAt(Globals.ProxyNum);

            OutputToFile.WriteLine($"Trying following proxy: {Globals.ProxyNum}");
            Globals.ProxyNum++;

            return randomProxy;
        }


        public static WebProxy Parse(string hostname, string port, string username, string password)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var parsedProxy = new WebProxy();

            var ip = Dns.GetHostAddresses(hostname); // will throw exception if it cant resolve IP

            parsedProxy.Address = new Uri("http://" + ip[0] + ":" + port);

            if (username != null)
            {
                parsedProxy.Credentials = new NetworkCredential(username, password);
            }

            return parsedProxy;
        }
    }
}
