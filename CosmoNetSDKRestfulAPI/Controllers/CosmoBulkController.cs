using CosmoNetSDKRestfulAPI.Models.CosmoDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CosmoBulkController : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        private readonly Container _Container;
        public CosmoBulkController(CosmosClient CosmosClient)
        {
            //Important if bulk exection should work
            CosmosClient.ClientOptions.AllowBulkExecution = true; 

            this._CosmoClient = CosmosClient;
            this._Container = CosmosClient.GetContainer("Families", "Families");
        }

        [HttpPost]
        public async Task<ActionResult> CreateBulk()
        {

            var cosmoObj = new List<CosmoFamiliy>()
            {
                new CosmoFamiliy(){ Id = Guid.NewGuid().ToString()},
                new CosmoFamiliy(){ Id = Guid.NewGuid().ToString()}

            };

            var tasks = new List<Task>();

            foreach (var item in cosmoObj)
            {
                var task = this._Container.CreateItemAsync(item, new PartitionKey(item.Id));
                tasks.Add(task.ContinueWith(t=> { 
                
                    if(t.Status == TaskStatus.Faulted)
                        Console.WriteLine("Instance crashed" + t);
                }));
            }

            await Task.WhenAll(tasks);

            return Ok();
        }
    }
}
