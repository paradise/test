using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using DBLayer.Properties;
using MongoDB.Driver.Builders;

namespace DBLayer
{
    public class MongoWorker
    {
        private MongoServer _server;
        private MongoDatabase _db;

        public MongoWorker()
        {
            MongoConnectionStringBuilder builder = new MongoConnectionStringBuilder();
            builder.Username = Settings.Default.MongoUser;
            builder.Password = Settings.Default.MongoPassword;
            builder.Server = new MongoServerAddress(Settings.Default.MongoServer, Convert.ToInt32(Settings.Default.MongoPort));
            _server = MongoServer.Create(builder);
            _db = _server.GetDatabase(Settings.Default.MongoDb);
        }

        public List<T> GetDocuments<T>(string collection, IMongoQuery query)
        {
            return _db.GetCollection<T>(collection).Find(query).ToList();
        }

        public T GetDocument<T>(string collection, IMongoQuery query)
        {
            return this.GetDocuments<T>(collection, query).FirstOrDefault();
        }

        public bool ExistsDocument<T>(string collection, IMongoQuery query)
        {
            return this.GetDocuments<T>(collection, query).Count > 0;
        }

        public SafeModeResult InsertDocument<T>(string collection, T document)
        {
            return _db.GetCollection<T>(collection).Insert<T>(document, SafeMode.True);
        }



    }
}
