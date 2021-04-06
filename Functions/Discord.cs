using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using Discord.Webhook;

namespace Aldi_Monitor.Functions
{
    class Discord
    {
        //public static async void NotifyDiscordAsync(MonitorTask monitorTask, List<string> availability)
        //{
        //    var embed = new EmbedBuilder();

        //    var embeds = new List<Embed>();

        //    //var message = availability.Aggregate("", (current, location) => current + "\n");

        //    var message = availability.Aggregate("", (current, store) => current + (store + "\n"));

        //    if (message.Length > 1024)
        //    {
        //        message = message.Substring(0, 994);
        //        message += "\n... And many others";
        //    }

        //    embeds.Add(embed
        //        .WithAuthor("New stock found on Argos!")
        //        .WithFooter("RYXX Monitors | @ryxxuk")
        //        .WithColor(Color.Blue)
        //        .WithTitle(monitorTask.product.itemName)
        //        .WithFields(new EmbedFieldBuilder
        //        {
        //            Name = "Postcode Checked:",
        //            Value = monitorTask.postcode
        //        })
        //        .WithFields(new EmbedFieldBuilder
        //        {
        //            Name = "Available at:",
        //            Value = message
        //        })
        //        .WithCurrentTimestamp()
        //        .WithThumbnailUrl(
        //            $"https://media.4rgos.it/s/Argos/{monitorTask.product.productSku}_R_SET?$Main768$&amp;w=620&amp;h=620")
        //        .WithUrl("https://www.argos.co.uk/product/" + monitorTask.product.productSku)
        //        .Build());


        //    foreach (var client in monitorTask.webhooks.Select(webhook => new DiscordWebhookClient(webhook)))
        //    {
        //        await client.SendMessageAsync("", false, embeds: embeds);
        //    }
        //}
    }
}
