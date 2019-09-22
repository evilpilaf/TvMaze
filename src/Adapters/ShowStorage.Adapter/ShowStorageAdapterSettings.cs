namespace ShowStorage.Adapter
{
    public sealed class ShowStorageAdapterSettings
    {
        public string AccountEndpoint { get; }
        public string AccountKey { get; }

        public ShowStorageAdapterSettings(string accountEndpoint, string accountKey)
        {
            AccountEndpoint = accountEndpoint;
            AccountKey = accountKey;
        }
    }
}
