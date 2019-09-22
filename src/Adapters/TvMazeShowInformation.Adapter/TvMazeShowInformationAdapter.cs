using Microsoft.Extensions.DependencyInjection;

using Scraper.Core.Adapters;

namespace TvMazeShowInformation.Adapter
{
    public class TvMazeShowInformationAdapter
    {
        public void Register(IServiceCollection hostContainer)
        {
            hostContainer.AddScoped<ITvMazeClient, TvMazeClient>();
        }
    }
}
