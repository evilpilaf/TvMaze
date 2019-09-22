using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Scraper.Core.Adapters;
using Scraper.Core.DomainExceptions;
using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace Scraper.Core.UseCases
{
    public class GetNewMoviesUseCase
    {
        private readonly IShowStore _showStore;
        private readonly ITvMazeClient _tvMazeClient;

        public GetNewMoviesUseCase(IShowStore showStore, ITvMazeClient tvMazeClient)
        {
            _showStore = showStore;
            _tvMazeClient = tvMazeClient;
        }

        public async Task<Result<Unit>> Execute()
        {
            const int batchSize = 1;

            int latestMovieId = await _showStore.GetLatestScrapedShowId();

            return await GetNextBatch(batchSize, latestMovieId);
        }

        private async Task<Result<Unit>> GetNextBatch(int batchSize, int initialId)
        {
            Task<Result<Show>> GetMovieInfoFromTvMaze(int id)
            {
                return _tvMazeClient.GetShowById(id);
            }

            var tasks = new List<Task<Result<Show>>>(batchSize);
            
            for (int i = 1; i <= batchSize; i++)
            {
                tasks.Add(GetMovieInfoFromTvMaze(initialId + i));
            }

            await Task.WhenAll(tasks);

            var moviesToStore = new List<Show>(batchSize);
            var errors = new List<Exception>(batchSize);

            foreach (var result in tasks)
            {
                var r = await result;
                r.Match(
                    succ: val =>
                    {
                        moviesToStore.Add(val);
                        return Unit.Default;
                    },
                    fail: ex =>
                    {
                        errors.Add(ex);
                        return Unit.Default;
                    });
            }

            await _showStore.StoreMultiple(moviesToStore);

            if (errors.Any(e => e is ThrottleException))
            {
                return errors.Single(e => e is ThrottleException);
            }

            return await GetNextBatch(batchSize, initialId + batchSize);
        }
    }
}
