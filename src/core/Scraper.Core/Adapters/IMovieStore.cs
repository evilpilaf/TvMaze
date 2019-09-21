using System.Collections.Generic;
using System.Threading.Tasks;

using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace Scraper.Core.Adapters
{
    public interface IMovieStore
    {
        Task<PageResult<Show>> GetAll(int pageSize, int pageNumber);
        Task<int> GetLatestScrapedMovieId();
        Task StoreMultiple(IReadOnlyList<Show> moviesToStore);
    }
}
