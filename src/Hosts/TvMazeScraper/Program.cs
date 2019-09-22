using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Scraper.Core.Adapters;
using Scraper.Core.UseCases;
using Scraper.Core.ValueTypes;
using Serilog;
using Serilog.Core;
using ShowStorage.Adapter;
using TvMazeScraper.Settings;
using TvMazeShowInformation.Adapter;


namespace TvMazeScraper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Log.Logger = new LoggerConfiguration()
                                .WriteTo
                                .Console()
                                .CreateLogger();

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var serviceCollection = new ServiceCollection()
                                        .AddLogging(c => c.AddSerilog(dispose: true));

            ConfigureApplication(configuration, serviceCollection);

            await Execute(serviceCollection);
        }

        private static async Task<Result<Unit>> Execute(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var useCase = serviceProvider.GetService<GetNewMoviesUseCase>();


            var result = await useCase.Execute();
            return result;
        }

        private static void ConfigureApplication(IConfigurationRoot configuration, IServiceCollection serviceCollection)
        {
            var apiSettings = configuration.GetSection("tvMazeApi").Get<TvMazeApiSettings>();
            var showDbSettings = configuration.GetSection("showsDatabaseSettings").Get<ShowDatabaseSettings>();


            serviceCollection.AddScoped<GetNewMoviesUseCase>();

            serviceCollection.AddHttpClient(nameof(ITvMazeClient),
                c => c.BaseAddress = new Uri(apiSettings.BaseUrl));

            var showStorageAdapter =
                new ShowStorageAdapter(new ShowStorageAdapterSettings(showDbSettings.CosmosDbAccountEndpoint,
                    showDbSettings.CosmosDbAccountKey));

            showStorageAdapter.Register(serviceCollection);

            var tvMazeScraperAdapter = new TvMazeShowInformationAdapter();
            tvMazeScraperAdapter.Register(serviceCollection);
        }
    }
}
