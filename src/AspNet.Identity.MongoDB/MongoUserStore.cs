using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB.Entities;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace AspNet.Identity.MongoDB 
{   
    public class MongoUserStore<TUser> where TUser : MongoIdentityUser 
    {
        private readonly MongoCollection<TUser> _mongoCollection;
        
        public MongoUserStore(MongoCollection<TUser> mongoCollection)
        {
            if (mongoCollection == null)
            {
                throw new ArgumentNullException("mongoCollection");
            }

            _mongoCollection = mongoCollection;
            
            EnsureIndexes();
        }
        
        // privates

        private void EnsureIndexes()
        {
            var emailKeyBuilder = new IndexKeysBuilder<TUser>().Ascending(user => user.Email.Value);
            var loginKeyBuilder = new IndexKeysBuilder<TUser>().Ascending(
                user => user.Logins.Select(login => login.LoginProvider), 
                user => user.Logins.Select(login => login.ProviderKey));

            _mongoCollection.CreateIndex(emailKeyBuilder, new IndexOptionsBuilder().SetUnique(true));
            _mongoCollection.CreateIndex(loginKeyBuilder);
        }

        private TResult Execute<TResult>(Func<TResult> func) where TResult : CommandResult
        {
            TResult result = func();
            if (result.Ok == false)
            {
                throw new MongoException(string.Format(
                    "Update operation was unsuccessful. Code: {0}, ErrorMessage: {1}",
                    result.Code,
                    result.ErrorMessage));
            }

            return result;
        }
    }
}