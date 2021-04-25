using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aldi_Monitor.Models;
using Discord;
using Discord.Webhook;

namespace Aldi_Monitor.Functions
{
    internal class Discord
    {
        public static async void NotifyDiscordAsync(Item item, int stock)
        {
            var embed = new EmbedBuilder();

            var embeds = new List<Embed>
            {
                embed
                    .WithAuthor("New stock found on Aldi!","https://cdn.aldi-digital.co.uk/32FDVWu4Lhbxgj9Z3v03ji0pGJIp?&w=70&h=84")
                    .WithFooter("RYXX Monitors | @ryxxuk")
                    .WithColor(Color.Blue)
                    .WithTitle(item.Name)
                    .WithFields(new EmbedFieldBuilder {Name = "Product Sku", Value = item.ProductSku, IsInline = true})
                    .WithFields(new EmbedFieldBuilder {Name = "Stock Available", Value = stock, IsInline = true})
                    .WithDescription($"Price: £{item.Price}")
                    .WithCurrentTimestamp()
                    .WithThumbnailUrl(item.ImageUrl)
                    .WithUrl($"https://www.aldi.co.uk/p/{item.ProductSku}")
                    .Build()
            };

            foreach (var client in item.Webhooks.Select(webhook => new DiscordWebhookClient(webhook)))
            {
                await client.SendMessageAsync("", false, embeds: embeds);
            }

            OutputToFile.Write("\nDISCORD PING SENT!");
        }
    }
}
