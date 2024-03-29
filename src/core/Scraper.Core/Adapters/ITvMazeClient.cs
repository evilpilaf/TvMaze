﻿using System.Threading.Tasks;

using Scraper.Core.Entities;
using Scraper.Core.ValueTypes;

namespace Scraper.Core.Adapters
{
    public interface ITvMazeClient
    {
        Task<Result<Show>> GetShowById(int showId);
    }
}
