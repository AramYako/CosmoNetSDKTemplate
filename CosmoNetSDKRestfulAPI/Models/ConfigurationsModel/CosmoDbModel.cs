using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Models.ConfigurationsModel
{
    public class CosmoDbModel
    {
        public const string Section =  "CosmoDbData";

        public string CosmoDbEndPoint { get; set; }
        public string CosmoMasterKey { get; set; }

    }
}