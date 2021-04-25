using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Models.CosmoDocuments
{
    public class CosmoFamiliy
    {

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "familiyName")]
        public string FamiliyName { get; set; }

        [JsonProperty(PropertyName = "address")]
        public CosmoAddress Address { get; set; }

        [JsonProperty(PropertyName = "parents")]
        public List<string> Parents { get; set; }

        [JsonProperty(PropertyName = "pk")]
        public string Pk { get; set; }

        public CosmoFamiliy()
        {
            Address = new CosmoAddress();
            Parents = new List<string>();
        }
    }
}
