using System.Threading.Tasks;

using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace Scraper.Core.Adapters
{
    public interface IMovieStore
    {
        Task<PageResult<Movie>> GetAll(int pageSize, int pageNumber);
    }
}
