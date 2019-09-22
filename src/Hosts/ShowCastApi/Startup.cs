using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Scraper.Core.UseCases;

using Serilog;

using ShowCastApi.Settings;

using ShowStorage.Adapter;

[assembly: FunctionsStartup(typeof(ShowCastApi.Startup))]
namespace ShowCastApi
{
    public class Startup : FunctionsStartup
    {
        private FunctionSettings _settings;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            _settings = config.Get<FunctionSettings>();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatString = "yyyy-MM-dd",
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };

            Log.Logger = new LoggerConfiguration()
                                .MinimumLevel.Debug()
                                .WriteTo.Console()
                                .CreateLogger();

            BuildContainer(builder.Services);

            builder.Services
                   .AddLogging(loggerBuilder => loggerBuilder.AddSerilog())
                   .BuildServiceProvider(validateScopes: true);
        }

        private void BuildContainer(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<GetPageOfShowsUseCase>();
            var showStorageAdapter =
                new ShowStorageAdapter(new ShowStorageAdapterSettings(_settings.CosmosDbAccountEndpoint,
                    _settings.CosmosDbAccountKey));

            showStorageAdapter.Register(serviceCollection);
        }
    }
}
