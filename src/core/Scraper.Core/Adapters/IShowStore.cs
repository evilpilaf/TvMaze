using System.Collections.Generic;
using System.Threading.Tasks;

using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace Scraper.Core.Adapters
{
    public interface IShowStore
    {
        Task<Result<PageResult<Show>>> GetAll(int pageSize, int pageNumber);
        Task<Result<int>> GetLatestScrapedShowId();
        Task<Result<Unit>> StoreMultiple(IReadOnlyList<Show> showsToStore);
    }
}
