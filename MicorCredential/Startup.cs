namespace MicroCredential
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MicroCredential.Services;
    using Microsoft.Azure.Cosmos;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos.Fluent;

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

            services.AddSingleton<ICosmosDatabaseService>(InitializeCosmosDatabaseInstance(Configuration.GetSection("Connectionstring")).GetAwaiter().GetResult());

            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankManagmentAPI");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static async Task<CosmosDatabaseService> InitializeCosmosDatabaseInstance(IConfigurationSection configurationSection)
        {
            string dataBaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string primaryKey = configurationSection.GetSection("PrimaryKey").Value;
            string endPointUrl = configurationSection.GetSection("EndpointUrl").Value;

            CosmosClientBuilder cosmosClientBuilder = new CosmosClientBuilder(endPointUrl, primaryKey);

            CosmosClient cosmosClient = cosmosClientBuilder.WithConnectionModeDirect().Build();

            CosmosDatabaseService cosmosDatabaseService = new CosmosDatabaseService(cosmosClient, dataBaseName, containerName);

            DatabaseResponse databaseResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync(dataBaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/ServiceType");
            return cosmosDatabaseService;
        }
    }
}
