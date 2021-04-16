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
        private int requestNum = 0;
        private int discordPings = 0;
        private int errors = 0;

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
               OutputToFile.WriteLine($"Starting new task!{item.Name} [{(item.UseProxy ? "USING PROXY" : "NOT USING PROXY")}]");
               StartMonitorTask(item);
               OutputToFile.WriteLine($"Sleeping 10 seconds!");
               Thread.Sleep(10*1000);
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

                itemsToBeMonitored.Add(item);
            }

            return itemsToBeMonitored;
        }

        public void StartMonitorTask(Item item)
        {
            OutputToFile.WriteLine($"Starting task for {item.Name}!");
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
                            discordPings++;

                            OutputToFile.WriteLine($"{item.Name} #INSTOCK NOTIFIYING DISCORD");
                            
                            Functions.Discord.NotifyDiscordAsync(item, response);
                        }
                        else
                        {
                            OutputToFile.WriteLine($"{item.Name} #STOCKUNCHANGED");
                        }

                        item.InStock = true;
                    }
                    else
                    {
                        OutputToFile.WriteLine($"{item.Name} #OUTOFSTOCK");
                        item.InStock = false;
                    }

                    requestNum++;
                    UpdateTitle();
                    Thread.Sleep(item.Interval);
                }
            }
            catch (Exception e)
            {
                OutputToFile.WriteLine(e.ToString());
                Thread.Sleep(120000);
                errors++;
                UpdateTitle();
                OutputToFile.WriteLine($"Slept 120 seconds. Restarting task for {item.Name}!");
                Task.Run(() => MonitorTask(item));
            }
        }
        public void UpdateTitle()
        {
            Console.Title = $"Requests Made: {requestNum} | Discord Pings: {discordPings} | Errors: {errors}";
        }
    }
}
