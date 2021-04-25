using CosmoNetSDKRestfulAPI.Models.CosmoDocuments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        private readonly Container _Container;
        public DocumentController(CosmosClient CosmosClient)
        {
            this._CosmoClient = CosmosClient;
            this._Container = CosmosClient.GetContainer("Families", "Families");
        }

        #region GET
        [HttpGet]
        public async Task<ActionResult<List<CosmoFamiliy>>> GetDocuments()
        {
            //Default it's set to 100
            QueryRequestOptions queryOptions = new QueryRequestOptions()
            {
                MaxItemCount = 50
            };

            var items = this._Container
                .GetItemLinqQueryable<CosmoFamiliy>
                (false, null, queryOptions)
                .ToFeedIterator();

            List<CosmoFamiliy> itemsList = new List<CosmoFamiliy>();

            //Get maxItemCount. Do we have more aftet the MaxItemCount returns true
            while (items.HasMoreResults)
            {
                var page = await items.ReadNextAsync();
                itemsList.AddRange(page.Resource);
            }

            return Ok(itemsList);

        }

        [HttpGet("{continuationToken}")]
        public async Task<ActionResult<(IEnumerable<CosmoFamiliy>, string)>> GetDocumentsPagination(string continuationToken)
        {
            //Default it's set to 100
            QueryRequestOptions queryOptions = new QueryRequestOptions()
            {
                MaxItemCount = 50
            };

            var items = this._Container
                .GetItemLinqQueryable<CosmoFamiliy>
                (false, continuationToken, queryOptions)
                .ToFeedIterator();

            var page = await items.ReadNextAsync();


            return Ok(new Tuple<IEnumerable<CosmoFamiliy>, string>(page.Resource, page.ContinuationToken));

        }


        [HttpGet("{id}/{partitionKey}")]
        public async Task<ActionResult<CosmoFamiliy>> GetDocument(string id, string partitionKey)
        {
            var requestOption = new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(partitionKey)
            };


            var result = await this._Container.
                GetItemLinqQueryable<CosmoFamiliy>(false, null, requestOption)
                .ToFeedIterator()
                .ReadNextAsync();

            return Ok(result.Resource);


        }
        //No partition key, hence will check all partitions
        [HttpGet("crossPartition/{id}")]
        public async Task<ActionResult<CosmoFamiliy>> GetDocument(string id)
        {
            var result = await this._Container.GetItemLinqQueryable<CosmoFamiliy>(false)
                .Where(f => f.Id == id)
                .ToFeedIterator()
                .ReadNextAsync();

            return Ok(result.Resource);

        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<CosmoFamiliy>> CreateDocument(CosmoFamiliy familiyModel)
        {
            familiyModel.Id = Guid.NewGuid().ToString();
            ItemResponse<CosmoFamiliy> response = await this._Container
                .CreateItemAsync
                (familiyModel, new PartitionKey(familiyModel.Address.ZipCode));

            return Ok(response);
        }

        #endregion

        #region PUT
        [HttpPut("{id}")]
        public async Task<ActionResult<CosmoFamiliy>> ReplaceDocument(string id, CosmoFamiliy familiyModel)
        {
            CosmoFamiliy result = await this._Container.ReplaceItemAsync<CosmoFamiliy>(
                familiyModel,
                id,
                new PartitionKey(familiyModel.Address.ZipCode));

            return Ok(result);
        }

        #endregion

        #region DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<CosmoFamiliy>> DeleteDocument(string id,string partitionKey)
        {
            ItemResponse<CosmoFamiliy> result = await this._Container.DeleteItemAsync<CosmoFamiliy>(id,
                new PartitionKey(partitionKey));

            return Ok(result);
        }
        #endregion
    }

}
