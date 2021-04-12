using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aldi_Monitor.Models;

namespace Aldi_Monitor.Functions
{
    public class Monitor
    {
        public static async Task<int> MonitorProduct(Item item)
        { 
            var response = await Aldi.GetAvailability(item.ProductSku);

            response = response.Replace(@"\", string.Empty);

            var inStockRegex = new Regex("isInStock\":(.*)}");

            var stockMatches = inStockRegex.Matches(response);

            var inStock = Convert.ToBoolean(stockMatches[0].Groups[1].Value);

            var comingSoonRegex = new Regex("showCommingSoonBanner\":(.*?),");

            var comingSoonMatches = comingSoonRegex.Matches(response);

            var comingSoon = Convert.ToBoolean(comingSoonMatches[0].Groups[1].Value);
            
            if (!inStock || comingSoon)
            {
                return 0;
            }

            var stockCountRegex = new Regex("data-stock-level=\"(.*?)\"");

            var stockCountMatches = stockCountRegex.Matches(response);

            var stockCount = Convert.ToInt32(stockCountMatches[0].Groups[1].Value);

            return stockCount;
        }
    }
}
