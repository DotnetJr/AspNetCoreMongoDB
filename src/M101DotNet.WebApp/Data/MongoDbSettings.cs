namespace M101DotNet.WebApp.Data
{
    public class MongoDbSettings
    {
        public string MongoDbConnection { get; set; }

        public string DbName { get; set; }

        public string PostsCollection { get; set; }

        public string UsersCollection { get; set; }
    }
}