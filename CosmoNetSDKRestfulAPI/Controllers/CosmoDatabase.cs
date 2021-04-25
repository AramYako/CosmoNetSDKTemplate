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
    public class CosmoDatabase : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        public CosmoDatabase(CosmosClient CosmosClient)
        {
            this._CosmoClient = CosmosClient;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDatabases()
        {
            FeedIterator<DatabaseProperties> databasesQuery = this._CosmoClient.GetDatabaseQueryIterator<DatabaseProperties>();

            FeedResponse<DatabaseProperties> databases = await databasesQuery.ReadNextAsync();

            return Ok(databases);

        }

        [HttpPost("{id}")]
        public async Task<ActionResult> CreateDatabase(string id)
        {
            DatabaseResponse databaseResponse = await this._CosmoClient.CreateDatabaseAsync(id);

            return Ok(databaseResponse.Resource);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDatabase(string id)
        {
            DatabaseResponse databaseResponse = await this._CosmoClient.GetDatabase(id).DeleteAsync();

            return Ok(databaseResponse);
        }

    }
}
