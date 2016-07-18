using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core
{
    [JsonObject]
    [Serializable]
    public sealed class Device
    {
        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public string Vendor { get; set; }
        [JsonProperty]
        public string IPAddress { get; set; }
        [JsonProperty]
        public string Description { get; set; }

        [JsonProperty]
        public string RemoteMAC { get; set; }
        [JsonProperty]
        public string RemoteAPID { get; set; }
        [JsonProperty]
        public string RemoteAEQualifier { get; set; }
        [JsonProperty]
        public string RemotePSelector { get; set; }
        [JsonProperty]
        public string RemoteSSelector { get; set; }
        [JsonProperty]
        public string RemoteTSelector { get; set; }

        [JsonProperty]
        public string LocalAPID { get; set; }
        [JsonProperty]
        public string LocalAEQualifier { get; set; }
        [JsonProperty]
        public string LocalPSelector { get; set; }
        [JsonProperty]
        public string LocalSSelector { get; set; }
        [JsonProperty]
        public string LocalTSelector { get; set; }
    }
}
