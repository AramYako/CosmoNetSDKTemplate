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
    public class TransactionController : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        private readonly Container _Container;
        public TransactionController(CosmosClient CosmosClient)
        {
            this._CosmoClient = CosmosClient;

            this._Container = CosmosClient.GetContainer("Families", "Families");
        }

        [HttpPost]
        public async Task ExecuteTransaction()
        {
            string PkValue = Guid.NewGuid().ToString();

            var cosmoObj = new List<CosmoFamiliy>()
            {
                new CosmoFamiliy(){ Id = Guid.NewGuid().ToString(), Pk=PkValue },

                new CosmoFamiliy(){ Id = Guid.NewGuid().ToString(),  Pk=PkValue}
            };

            TransactionalBatch batch = this._Container.CreateTransactionalBatch(new PartitionKey(PkValue));
          
            foreach(var item in cosmoObj)
                batch.CreateItem(item);

            //we can also replace
            //foreach (var item in cosmoObj)
                //batch.ReplaceItem(item.Id, item);

            var response = await batch.ExecuteAsync();

            if (response.IsSuccessStatusCode)
            {
                // The transaction success
            }
            else
            {
                Console.WriteLine("transaction failed");
                foreach (var item in response)
                    Console.WriteLine(item.StatusCode);
            }

        }
    }
}
