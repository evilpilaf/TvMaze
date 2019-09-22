using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.Azure.Cosmos;

using Scraper.Core.Adapters;
using Scraper.Core.DomainExceptions;
using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

using Serilog;

using ShowStorage.Adapter.Dtos;

namespace ShowStorage.Adapter
{
    internal sealed class ShowStoreCosmosDb : IShowStore
    {
        private readonly ILogger _logger = Log.Logger;
        private readonly CosmosClient _cosmosClient;

        public ShowStoreCosmosDb(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<Result<PageResult<Show>>> GetAll(int pageSize, int pageNumber)
        {
            string query = $"SELECT * FROM c OFFSET {(pageNumber - 1) * pageSize} LIMIT {pageSize}";
            var container = GetContainer();

            var queryDefinition = new QueryDefinition(query);

            try
            {
                _logger.Information("Attempting to retrieve all shows.");
                FeedIterator<ShowCosmosDto> feedIterator =
                    container.GetItemQueryIterator<ShowCosmosDto>(queryDefinition);

                var results = new List<ShowCosmosDto>(pageSize);
                while (feedIterator.HasMoreResults)
                {
                    var currentResultSet = await feedIterator.ReadNextAsync();
                    results.AddRange(currentResultSet.Resource);
                }

                return new PageResult<Show>(pageNumber, pageSize, results.Select(r => r.ToEntity()).ToList());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when retrieving shows");
                return new PersistenceException("Exception when retrieving shows", ex);
            }
        }

        public async Task<Result<int>> GetLatestScrapedShowId()
        {
            const string query = "SELECT VALUE MAX(c.tvmazeid) FROM c";
            var container = GetContainer();

            FeedIterator<int> resultSetIterator = container.GetItemQueryIterator<int>(query,
                requestOptions: new QueryRequestOptions { MaxItemCount = 1 });
            var results = new List<int>(1);
            while (resultSetIterator.HasMoreResults)
            {
                results.AddRange(await resultSetIterator.ReadNextAsync());
            }

            return results.Max();
        }

        public async Task<Result<Unit>> StoreMultiple(IReadOnlyList<Show> showsToStore)
        {
            var container = GetContainer();
            try
            {
                foreach (var show in showsToStore)
                {
                    var dto = new ShowCosmosDto(show);
                    ItemResponse<ShowCosmosDto> result = await container.CreateItemAsync(dto);
                    if (result.StatusCode != HttpStatusCode.OK)
                    {
                        _logger.Error(
                            "Unsuccessful storage of show {show}, received response status code {resultStatusCode}",
                            show.Id,
                            result.StatusCode);
                    }
                }

                return Unit.Default;
            }
            catch (Exception ex)
            {
                return new PersistenceException(ex.Message, ex);
            }
        }

        private Container GetContainer()
            => _cosmosClient.GetContainer("Rtl", "Shows");
    }
}
