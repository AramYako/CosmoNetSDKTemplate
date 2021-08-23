using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProcedureController : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        private readonly Container _Container;
        public StoredProcedureController(CosmosClient CosmosClient)
        {
            //Important if bulk exection should work
            CosmosClient.ClientOptions.AllowBulkExecution = true;

            this._CosmoClient = CosmosClient;

            this._Container = CosmosClient.GetContainer("Families", "Families");
        }

        [HttpGet]
        public async Task<ActionResult> GetAllSp()
        {

            var prc = this._Container.Scripts.GetStoredProcedureQueryIterator<StoredProcedureProperties>();

            var sprocs = await prc.ReadNextAsync();

            return Ok($"Found this many sp {sprocs.Count} RU: {sprocs.RequestCharge}");
        }


        #region Create SP
        [HttpPost("{sprocId}")]
        public async Task<ActionResult> CreateStoredPc(string sprocId)
        {
            sprocId = "SpHelloWorld";

            var sprocBody = System.IO.File.ReadAllText($@"Server\{sprocId}.js");



            var sprocProps = new StoredProcedureProperties
            {
                Id = sprocId,
                Body = sprocBody
            };

            var result = await this._Container.Scripts.CreateStoredProcedureAsync(sprocProps);

            return Ok($"Sp created, request charge {result.RequestCharge} RUs");
        }
        [HttpPost]
        public async Task<ActionResult> CreateSpCreateDocument()
        {
            string sprocId = "spCreateDocument";

            var sprocBody = System.IO.File.ReadAllText($@"Server\{sprocId}.js");



            var sprocProps = new StoredProcedureProperties
            {
                Id = sprocId,
                Body = sprocBody
            };

            var result = await this._Container.Scripts.CreateStoredProcedureAsync(sprocProps);

            return Ok($"Sp created, request charge {result.RequestCharge} RUs");
        }
        #endregion


        #region POSt

        [HttpPost("/Document")]
        public async Task<ActionResult> CreateDocument()
        {
            var id = Guid.NewGuid().ToString();

            dynamic documentDefinition = new
            {
                id = id,
                name = "demo document",
                type = "pdf",
                address = new 
                {
                    zipCode =  "111"
                }
            };

            var pk = new PartitionKey("111");
            var result = await this._Container.Scripts.ExecuteStoredProcedureAsync<dynamic>("spCreateDocument", pk, new[] { documentDefinition, true });

            return Ok("Document created, RU " + result.RequestCharge);
        }

        #endregion


    }
}
