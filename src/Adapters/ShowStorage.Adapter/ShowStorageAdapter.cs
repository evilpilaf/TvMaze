using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

using Scraper.Core.Adapters;

using SimpleInjectorContainer = SimpleInjector.Container;

namespace ShowStorage.Adapter
{
    public class ShowStorageAdapter
    {
        private readonly SimpleInjectorContainer _container;

        public ShowStorageAdapter(ShowStorageAdapterSettings settings)
        {
            _container = new SimpleInjectorContainer();
            _container.Register<CosmosClient>(() =>
                new CosmosClient(settings.AccountEndpoint, settings.AccountKey));
            _container.Register<IShowStore, ShowStoreCosmosDb>();
        }

        public void Register(IServiceCollection hostContainer)
        {
            hostContainer.AddScoped(_ => _container.GetInstance<IShowStore>());
        }
    }
}
