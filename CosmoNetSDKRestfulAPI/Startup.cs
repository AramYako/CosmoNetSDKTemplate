using CosmoNetSDKRestfulAPI.Models.ConfigurationsModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmoNetSDKRestfulAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CosmoNetSDKRestfulAPI", Version = "v1" });
            });

            CosmoDbModel cosmoModel = new CosmoDbModel();
            Configuration.GetSection(CosmoDbModel.Section).Bind(cosmoModel);

            CosmosClient cosmoClient = new CosmosClient(cosmoModel.CosmoDbEndPoint, cosmoModel.CosmoMasterKey);

            services.AddSingleton(s =>
            {
                var connectionString = Configuration["CosmosDBConnection"];
                if (string.IsNullOrEmpty(cosmoModel?.CosmoDbEndPoint))
                {
                    throw new InvalidOperationException(
                        "Please specify a valid CosmosDBConnection in the appSettings.json file or your Azure Functions Settings.");
                }
                if (string.IsNullOrEmpty(cosmoModel?.CosmoMasterKey))
                {
                    throw new InvalidOperationException(
                        "Please specify a valid CosmoMasterKey in the appSettings.json file or your Azure Functions Settings.");
                }
                //CosmosClient is thread-safe. Its recommended to maintain a single instance of CosmosClient per lifetime
                return new CosmosClient(cosmoModel.CosmoDbEndPoint, cosmoModel.CosmoMasterKey);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CosmoNetSDKRestfulAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
