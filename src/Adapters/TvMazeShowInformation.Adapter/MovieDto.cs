using System;

namespace TvMazeShowInformation.Adapter
{
    internal class ShowDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

    internal class CastDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
