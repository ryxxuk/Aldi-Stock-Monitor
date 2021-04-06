using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Aldi_Monitor.Functions;
using Aldi_Monitor.Models;
using Newtonsoft.Json.Linq;

namespace Aldi_Monitor
{
    internal class Program
    {
        static void Main()
        {
            var app = new Program();
            app.Start();
            Console.ReadLine();
        }

        private void Start()
        {
           var itemsToBeMonitored= GetAllItemsToBeMonitored();

           foreach (var item in itemsToBeMonitored)
           {
               StartMonitorTask(item);
           }
        }

        private IEnumerable<Item> GetAllItemsToBeMonitored()
        {
            var itemsToBeMonitored = new List<Item>();

            dynamic responseObject = JObject.Parse(FormatJson(File.ReadAllText(Directory.GetCurrentDirectory() + @"\items.json")));

            for (var i = 0; i < responseObject.items.Count; i++)
            {
                string productSku = responseObject.items[i].productSku;
                int interval = responseObject.items[i].interval * 1000;
                bool useProxy = responseObject.items[i].useProxy;

                var item = new Item
                {
                    ProductSku = productSku,
                    Interval = interval,
                    UseProxy = useProxy
                };

                itemsToBeMonitored.Add(item);
            }


            return itemsToBeMonitored;
        }

        public void StartMonitorTask(Item item)
        {
            Task.Run(() => Monitor.MonitorProduct(item));
        }

        private static string FormatJson(string json)
        {
            json = json.Trim().Replace("\r", string.Empty);
            json = json.Trim().Replace("\n", string.Empty);
            json = json.Replace(Environment.NewLine, string.Empty);

            return json;
        }
    }
}
