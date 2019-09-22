using System.Threading.Tasks;

using Scraper.Core.Adapters;
using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace Scraper.Core.UseCases
{
    public class GetPageOfShowsUseCase
    {
        private readonly IShowStore _showStore;

        public GetPageOfShowsUseCase(IShowStore showStore)
        {
            _showStore = showStore;
        }

        public Task<Result<PageResult<Show>>> Execute(int pageSize = 10, int pageNumber = 1)
        {
            return _showStore.GetAll(pageSize, pageNumber);
        }
    }
}
