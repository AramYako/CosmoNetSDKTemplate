using CosmoNetSDKRestfulAPI.Models.DTOs.Container;
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
    public class CosmoContainer : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        public CosmoContainer(CosmosClient CosmosClient)
        {
            this._CosmoClient = CosmosClient;
        }

        [HttpGet("{databaseId}")]
        public async Task<ActionResult> GetContainers(string databaseId)
        {
            Database database = this._CosmoClient.GetDatabase(databaseId);
            
            FeedResponse<ContainerProperties> containers = await database
                .GetContainerQueryIterator<ContainerProperties>()
                .ReadNextAsync();

            return Ok(containers.Resource);

        }

        [HttpGet("{databaseId}/{containerId}")]
        public async Task<ActionResult> GetContainersThroughPut(string databaseId, string containerId)
        {
            Database database = this._CosmoClient.GetDatabase(databaseId);

            if (database == null)
                return NotFound();

            FeedResponse<ContainerProperties> containers = await database
                .GetContainerQueryIterator<ContainerProperties>()
                .ReadNextAsync();

            List<ContainerDTO> containersDTO = new List<ContainerDTO>();

            //All containers for a database 
            foreach(var container in containers)
            {
                var containerForDb = this._CosmoClient.GetContainer(databaseId, container.Id);
                int? ThroughtPutValue = await containerForDb.ReadThroughputAsync();

                containersDTO.Add(new ContainerDTO()
                {
                    Id = container.Id,
                    LastModifyDate = container.LastModified,
                    PartitionKeyPath = container.PartitionKeyPath,
                    ThroughPut = ThroughtPutValue
                });
            }

            return Ok(containersDTO);

        }


        //Create container for a database
        [HttpPost("{throughtPut:int}/{partitionKey}")]
        public async Task<ActionResult> CreateContainer(string databaseId,string containerId, int throughtPut, string partitionKey)
        {
            //partitionKey => /address/zipcode

            partitionKey = partitionKey.Replace("%2F", "/");

            Database database = this._CosmoClient.GetDatabase(databaseId);

            FeedResponse<ContainerProperties> containers = await database
                .GetContainerQueryIterator<ContainerProperties>()
                .ReadNextAsync();

            if (containers.Resource.Any(c => c.Id.Equals(containerId, StringComparison.InvariantCultureIgnoreCase)))
                return BadRequest("Container already exists");

            ContainerProperties containerProp = new ContainerProperties()
            {
                Id = containerId,
                PartitionKeyPath = partitionKey
            };
            ContainerResponse response = await database.CreateContainerAsync(containerProp, throughtPut);

            return Ok(response);

        }

        [HttpDelete("{databaseId}/{containerId}")]
        public async Task<ActionResult> DeleteContainer(string databaseId, string containerId)
        {
            Container container = this._CosmoClient.GetContainer(databaseId, containerId);

            return Ok(await container.DeleteContainerAsync());

        }

    }
}
