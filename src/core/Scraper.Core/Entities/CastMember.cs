using NodaTime;

namespace Scraper.Core.Entities
{
    public readonly struct CastMember
    {
        public int Id { get; }
        public string Name { get; }
        public LocalDate? BirthDate { get; }

        public CastMember(int id, string name, LocalDate? birthDate)
        {
            Id = id;
            Name = name;
            BirthDate = birthDate;
        }
    }
}
