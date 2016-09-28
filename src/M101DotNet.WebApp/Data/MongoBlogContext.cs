using MongoDB.Driver;
using M101DotNet.WebApp.Models;

namespace M101DotNet.WebApp.Data
{
    public class MongoBlogContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly string _postsCollectionName;
        private readonly string _usersCollectionName;
        

        public MongoBlogContext(MongoDbSettings settings)
        {
            _client = new MongoClient(settings.MongoDbConnection);
            _database = _client.GetDatabase(settings.DbName);
            _postsCollectionName = settings.PostsCollection;
            _usersCollectionName = settings.UsersCollection;
        }

        public IMongoClient Client
        {
            get { return _client; }
        }

        public IMongoCollection<Post> Posts
        {
            get { return _database.GetCollection<Post>(_postsCollectionName); }
        }

        public IMongoCollection<MongoUser> Users
        {
            get { return _database.GetCollection<MongoUser>(_usersCollectionName); }
        }
    }
}