using System;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNet.Identity.MongoDB.Entities
{
    public class MongoUserPhoneNumber : MongoUserContactRecord
    {
        [BsonConstructor]
        private MongoUserPhoneNumber() : base(null)
        {
        }
        
        public MongoUserPhoneNumber(string phoneNumber) : base(phoneNumber)
        {
            if (phoneNumber == null) throw new ArgumentNullException("phoneNumber");
        }
    }
}