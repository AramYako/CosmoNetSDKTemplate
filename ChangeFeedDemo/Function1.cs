using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChangeFeedDemo
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([CosmosDBTrigger(
            databaseName: "Families",
            collectionName: "Families",
            ConnectionStringSetting = "CosmoDbConnectionString",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> documents, ILogger log)
        {
            log.LogInformation("Documents modified " + documents.Count);

            foreach (var document in documents)
            {
                var item = JsonConvert.DeserializeObject<dynamic>(document.ToString());
                log.LogInformation($"document with id {item.id} has benn updated or created");
            }

        }
    }
}
