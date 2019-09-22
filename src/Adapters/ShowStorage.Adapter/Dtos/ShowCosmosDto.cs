using System;
using System.Collections.Generic;
using System.Linq;

using NodaTime.Extensions;

using Scraper.Core.Entities;

namespace ShowStorage.Adapter.Dtos
{
    internal class ShowCosmosDto
    {
        public int TvMazeId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<CastCosmosDto> Cast { get; set; }

        public ShowCosmosDto(Show entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            TvMazeId = entity.TvMazeId;
            Cast = entity.Cast.Select(e => new CastCosmosDto(e));
        }

        public Show ToEntity()
        {
            return new Show(Id, Name, TvMazeId, Cast.Select(c => c.ToEntity()));
        }
    }

    internal class CastCosmosDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public CastCosmosDto(CastMember e)
        {
            Id = e.Id;
            Name = e.Name;
            BirthDate = e.BirthDate.ToDateTimeUnspecified();
        }

        public CastMember ToEntity()
        {
            return new CastMember(Id, Name, BirthDate.ToLocalDateTime().Date);
        }
    }
}
