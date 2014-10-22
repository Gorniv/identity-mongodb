using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNet.Identity.MongoDB.Entities
{
    public class MongoUserEmail : MongoUserContactRecord
    {
        [BsonConstructor]
        private MongoUserEmail() : base(null)
        {    
        }

        public MongoUserEmail(string email) : base(email)
        {
            if (email == null) throw new ArgumentNullException("email");
        }
    }
}