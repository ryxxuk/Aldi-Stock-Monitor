using System;
using System.Collections.Generic;
using System.Text;

namespace Aldi_Monitor.Models
{
    public class Item
    {
        public string ProductSku { get; set; }
        public bool UseProxy { get; set; }
        public int Interval { get; set; }
        public List<String> Webhooks { get; set; }
    }
}
