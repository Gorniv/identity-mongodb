using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AspNet.Identity.MongoDB.Entities 
{
    public class MongoIdentityUser
    {
        [BsonConstructor]
        private MongoIdentityUser()
        {
        }

        public MongoIdentityUser(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }

            UserName = userName;
        }

        public MongoIdentityUser(string userName, string email)
            : this(userName)
        {
            if (email == null)
            {
                throw new ArgumentNullException("email");
            }

            Email = new MongoUserEmail(email);
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public MongoUserEmail Email { get; set; }

        public MongoUserPhoneNumber PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsLockoutEnabled { get; set; }
        public bool IsTwoFactorEnabled { get; set; }

        public IEnumerable<MongoUserClaim> Claims { get; set; }
        public IEnumerable<MongoUserLogin> Logins { get; set; }

        public int AccessFailedCount { get; set; }
        public DateTime? LockoutEndDate { get; set; }
    }
}