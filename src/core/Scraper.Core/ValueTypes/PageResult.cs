using System.Collections.Generic;

namespace Scraper.Core.ValueTypes
{
    public readonly struct PageResult<T>
    {
        public int PageNumber { get; }
        public int PageSize { get; }
        public IReadOnlyList<T> Results { get; }

        public PageResult(int pageNumber, int pageSize, IReadOnlyList<T> results)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Results = results;
        }
    }
}
