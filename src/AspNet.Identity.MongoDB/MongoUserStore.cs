using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB.Entities;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace AspNet.Identity.MongoDB 
{   
    public class MongoUserStore<TUser> : IUserStore<TUser>,
        IUserLoginStore<TUser> where TUser : MongoIdentityUser
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
        
        // IUserStore
            
        public virtual Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            return Task.FromResult(user.Id);
        }
            
        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            return Task.FromResult(user.UserName);
        }
            
        public virtual Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            IMongoQuery updateQuery = Query<TUser>.EQ(u => u.Id, user.Id);
            UpdateBuilder<TUser> updateStatement = Update<TUser>.Set(usr => usr.UserName, userName);
            Execute(() => _mongoCollection.Update(updateQuery, updateStatement));
            user.UserName = userName;
            
            return Task.FromResult(0);
        }
            
        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            return Task.FromResult(user.NormalizedUserName);
        }
            
        public virtual Task SetNormalizedUserNameAsync(TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            IMongoQuery updateQuery = Query<TUser>.EQ(u => u.Id, user.Id);
            UpdateBuilder<TUser> updateStatement = Update<TUser>.Set(usr => usr.NormalizedUserName, userName);
            Execute(() => _mongoCollection.Update(updateQuery, updateStatement));
            user.NormalizedUserName = userName;
            
            return Task.FromResult(0);
        }
            
        public virtual Task CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            Execute(() => _mongoCollection.Insert(user));
            
            return Task.FromResult(0);
        }
            
        public virtual Task UpdateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            Execute(() => _mongoCollection.Save(user));

            return Task.FromResult(0);
        }
            
        public virtual Task DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            IMongoQuery removeQuery = Query<TUser>.EQ(u => u.Id, user.Id);
            Execute(() => _mongoCollection.Remove(removeQuery, RemoveFlags.Single));

            return Task.FromResult(0);
        }
        
        public virtual Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(userId == null)
            {
                throw new ArgumentNullException("userId");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            TUser user = _mongoCollection.FindOneByIdAs<TUser>(userId);
            
            return Task.FromResult(user);
        }
            
        public virtual Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(normalizedUserName == null)
            {
                throw new ArgumentNullException("normalizedUserName");
            }
            
            cancellationToken.ThrowIfCancellationRequested();
            
            IMongoQuery query = Query<TUser>.EQ(u => u.NormalizedUserName, normalizedUserName);
            TUser user = _mongoCollection.FindOneAs<TUser>(query);

            return Task.FromResult(user);
        }
            
        // IUserLoginStore
            
        public virtual Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
            
        public virtual Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
            
        public virtual Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
        
        public virtual Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
        
        // IDisposable

        public void Dispose()
        {
        }
            
        // privates

        private void EnsureIndexes()
        {
            var normalizedUserNameKeyBuilder = new IndexKeysBuilder<TUser>().Ascending(user => user.NormalizedUserName);
            var emailKeyBuilder = new IndexKeysBuilder<TUser>().Ascending(user => user.Email.Value);
            var loginKeyBuilder = new IndexKeysBuilder<TUser>().Ascending(
                user => user.Logins.Select(login => login.LoginProvider), 
                user => user.Logins.Select(login => login.ProviderKey));

            _mongoCollection.CreateIndex(normalizedUserNameKeyBuilder, new IndexOptionsBuilder().SetUnique(true));
            _mongoCollection.CreateIndex(emailKeyBuilder, new IndexOptionsBuilder().SetUnique(true));
            _mongoCollection.CreateIndex(loginKeyBuilder);
        }

        private TResult Execute<TResult>(Func<TResult> func) where TResult : CommandResult
        {
            TResult result;
            try
            {
                result = func();
            }
            catch
            {
                // TODO: log here.
                throw;
            }
            
            return result;
        }
    }
}