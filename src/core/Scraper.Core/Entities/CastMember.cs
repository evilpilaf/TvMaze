using System;

namespace Scraper.Core.Entities
{
    public readonly struct CastMember
    {
        public int Id { get; }
        public string Name { get; }
        public DateTimeOffset? BirthDate { get; }

        public CastMember(int id, string name, DateTimeOffset? birthDate)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
        }
    }
}
