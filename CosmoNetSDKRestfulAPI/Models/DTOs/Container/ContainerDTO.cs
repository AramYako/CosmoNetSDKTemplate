using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Models.DTOs.Container
{
    public class ContainerDTO
    {
        public string Id { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public string PartitionKeyPath { get; set; }
        public int? ThroughPut  { get; set; }
    }
}
