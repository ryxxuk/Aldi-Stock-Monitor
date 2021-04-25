using System.Collections.Generic;
using System.Net;

namespace Aldi_Monitor.Models
{
    internal class Globals
    {
        public static int RequestNum = 0;
        public static int DiscordPings = 0;
        public static int Errors = 0;
        public static List<WebProxy> Proxies = new List<WebProxy>();
        public static int ProxyNum = 0;
    }
}
