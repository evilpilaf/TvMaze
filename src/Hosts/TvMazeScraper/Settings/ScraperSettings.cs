namespace TvMazeScraper.Settings
{
    public class ScraperSettings
    {
        public TvMazeApiSettings TvMazeSettings { get; set; }
        public ShowDatabaseSettings ShowDbSettigns { get; set; }
    }

    public class TvMazeApiSettings
    {
        public string BaseUrl { get; set; }
    }

    public class ShowDatabaseSettings
    {
        public string CosmosDbAccountEndpoint { get; set; }
        public string CosmosDbAccountKey { get; set; }
    }
}
