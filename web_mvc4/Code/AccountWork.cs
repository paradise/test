using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBLayer;
using Models;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using web_mvc4.Models;

namespace web_mvc4.Code
{
    public class AccountWork
    {
        public static bool Auth(LoginModel model)
        {
            MongoWorker _mongo = new MongoWorker();
            var doc = _mongo.GetDocument<User>(MongoCollections.UserCollection, Query.EQ("Email", model.Email));
            if (doc != null)
            {
                string hash = Encoding.UTF8.GetString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(doc.ID.ToString() + model.Password)));
                if (hash == doc.Password)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Register(RegisterModel model)
        {
            MongoWorker _mongo = new MongoWorker();
            if (!_mongo.ExistsDocument<User>(MongoCollections.UserCollection, Query.EQ("Email", model.Email)))
            {
                ObjectId id = ObjectId.GenerateNewId();
                User user = new User()
                {
                    DisplayName = model.DisplayName,
                    Email = model.Email,
                    ID = id,
                    Password = Encoding.UTF8.GetString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(id.ToString() + model.Password)))
                };
                var resp = _mongo.InsertDocument<User>(MongoCollections.UserCollection, user);
                return resp.Ok;
            }
            else
            {
                return false;
            }
        }
    }
}