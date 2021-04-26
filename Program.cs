using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aldi_Monitor.Functions;
using Aldi_Monitor.Models;
using Newtonsoft.Json.Linq;
using Monitor = Aldi_Monitor.Functions.Monitor;

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

        private async Task Start()
        {
           var itemsToBeMonitored= await GetAllItemsToBeMonitored();

           foreach (var item in itemsToBeMonitored)
           {
               LoggingService.WriteLine($"Starting new task!{item.Name} [{(item.UseProxy ? "USING PROXY" : "NOT USING PROXY")}]");
               StartMonitorTask(item);
               LoggingService.WriteLine($"Sleeping 3 seconds!");
               Thread.Sleep(3*1000);
           }
        }

        private async Task<List<Item>> GetAllItemsToBeMonitored()
        {
            var itemsToBeMonitored = new List<Item>();

            dynamic responseObject = JObject.Parse(FormatJson(await File.ReadAllTextAsync(Directory.GetCurrentDirectory() + @"\items.json")));

            for (var i = 0; i < responseObject.items.Count; i++)
            {
                var item = new Item
                {
                    ProductSku = responseObject.items[i].productSku,
                    Interval = responseObject.items[i].interval * 1000,
                    UseProxy = responseObject.items[i].useProxy,
                    ImageUrl = responseObject.items[i].image,
                    Price = responseObject.items[i].price,
                    Name = responseObject.items[i].name,
                    InStock = false
                };

                var webhooks = new List<string>();

                for (var w = 0; w < responseObject.items[i].webhooks.Count; w++)
                {
                    webhooks.Add(responseObject.items[i].webhooks[w].url.ToString());
                }

                item.Webhooks = webhooks;

                if (item.UseProxy)
                {
                    item.Proxy = Proxy.GetNewProxy();
                }

                itemsToBeMonitored.Add(item);
            }

            return itemsToBeMonitored;
        }

        public void StartMonitorTask(Item item)
        {
            LoggingService.WriteLine($"Starting task for {item.Name}!");
            Task.Run(() => MonitorTask(item));
        }

        private static string FormatJson(string json)
        {
            json = json.Trim().Replace("\r", string.Empty);
            json = json.Trim().Replace("\n", string.Empty);
            json = json.Replace(Environment.NewLine, string.Empty);

            return json;
        }

        public async Task MonitorTask(Item item)
        {
            try
            {
                while (true)
                {
                    var response = await Monitor.MonitorProduct(item);

                    if (response > 0)
                    {
                        if (!item.InStock)
                        {
                            Globals.DiscordPings++;

                            LoggingService.WriteLine($"{item.Name} #INSTOCK NOTIFIYING DISCORD");
                            
                            Functions.Discord.NotifyDiscordAsync(item, response);
                        }
                        else
                        {
                            LoggingService.WriteLine($"{item.Name} #STOCKUNCHANGED");
                        }

                        item.InStock = true;
                    }
                    else
                    {
                        LoggingService.WriteLine($"{item.Name} #OUTOFSTOCK");
                        item.InStock = false;
                    }

                    Globals.RequestNum++;
                    UpdateTitle();
                    Thread.Sleep(item.Interval);
                }
            }
            catch (Exception e)
            {
                LoggingService.WriteLine(e.ToString());
                Thread.Sleep(120000);
                Globals.Errors++;
                UpdateTitle();
                LoggingService.WriteLine($"Slept 120 seconds. Restarting task for {item.Name}!");
                Task.Run(() => MonitorTask(item));
            }
        }
        public void UpdateTitle()
        {
            Console.Title = $"[Aldi] Requests: {Globals.RequestNum} | Pings: {Globals.DiscordPings} | Errors: {Globals.Errors}";
        }
    }
}
