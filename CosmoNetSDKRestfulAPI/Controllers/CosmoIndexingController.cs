using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //OBS by default all tags are being indexed except for etag
    //Check container => Scale & Settings => Indexing policy
    public class CosmoIndexingController : ControllerBase
    {
        private readonly CosmosClient _CosmoClient;
        private readonly Container _Container;
        public CosmoIndexingController(CosmosClient CosmosClient)
        {
            this._CosmoClient = CosmosClient;
            this._Container = CosmosClient.GetContainer("Families", "Families");
        }

        [HttpGet]
        public async Task<ActionResult<ContainerResponse>> InitContainer()
        {
            ContainerResponse response =  await this._Container.ReadContainerAsync();

            ContainerProperties containerProp =  response.Resource;

            containerProp.IndexingPolicy.IndexingMode = IndexingMode.Consistent;

            //does not index on parents 
            containerProp.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath() { Path = "/familiyName/*" });


            //Now we can do query Order by familiyname, address.city
            containerProp.IndexingPolicy.CompositeIndexes.Add(new Collection<CompositePath>
            {
                new CompositePath{Path = "/familiyName", Order = CompositePathSortOrder.Ascending},
                new CompositePath{Path = "/address/city", Order = CompositePathSortOrder.Ascending},
            });



            ContainerResponse responseAfterUpdate = await this._Container.ReplaceContainerAsync(containerProp);

            return Ok(responseAfterUpdate);
        }
    }
}
