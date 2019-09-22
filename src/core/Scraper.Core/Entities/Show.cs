using System;
using System.Collections.Generic;
using System.Linq;

namespace Scraper.Core.Entities
{
    public class Show
    {
        public Guid Id { get; }
        public string Name { get; }
        public int TvMazeId { get; }
        public IReadOnlyList<CastMember> Cast { get; }

        public Show(Guid id, string name, int tvMazeId, IEnumerable<CastMember> cast)
        {
            Id = id;
            Name = name;
            TvMazeId = tvMazeId;
            Cast = cast.OrderBy(c => c.BirthDate).ToList();
        }
    }
}
