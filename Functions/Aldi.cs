using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aldi_Monitor.Models;

namespace Aldi_Monitor.Functions
{
    class Aldi
    {
        public static async Task<string> GetAvailability(string sku)
        {
            try
            {
                var url = $"https://www.aldi.co.uk/api/product/availability/{sku}";

                using var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                };

                using var client = new HttpClient(handler);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url)
                };

                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36");

                var response = await client.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                OutputToFile.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
