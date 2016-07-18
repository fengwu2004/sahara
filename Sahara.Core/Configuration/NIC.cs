using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [Serializable]
    [JsonObject]
    public sealed class NIC
    {
        [JsonProperty]
        public string ID { get; set; }
        [JsonProperty]
        public string IP { get; set; }
    }
}
