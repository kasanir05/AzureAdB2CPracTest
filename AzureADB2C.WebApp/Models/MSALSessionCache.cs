using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;

namespace AzureADB2C.WebApp.Models
{
    public class MSALSessionCache
    {
        private static ReaderWriterLockSlim sessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        string userId = string.Empty;
        string CacheId = string.Empty;

        HttpContext localHttpContext = null;

        TokenCache cache = new TokenCache();

        public MSALSessionCache(string loggedUserId,HttpContext httpContext)
        {
            userId = loggedUserId;
            CacheId = userId + "_TokenCache";
            localHttpContext = httpContext;
        }

        public TokenCache GetMSALCacheInstance()
        {
            cache.BeforeAccess = BeforeAccessNotifications;
            cache.AfterAccess = AfterAccessNotifications;
            Load();
            return cache;
        }

        public void PersistCache()
        {
            sessionLock.EnterWriteLock();

            cache.HasStateChanged = false;

            localHttpContext.Session.Set(CacheId, cache.Serialize());

            sessionLock.ExitWriteLock();
        }

        public void SetUserState(string state)
        {
            sessionLock.EnterWriteLock();
            localHttpContext.Session.SetString(CacheId + "_state", state);
            sessionLock.ExitWriteLock();
        }

        public string ReadUserState()
        {
            sessionLock.EnterReadLock();
            string state = localHttpContext.Session.GetString(CacheId + "_state");
            sessionLock.ExitReadLock();
            return state;
        }

        public void Load()
        {
            sessionLock.EnterReadLock();
            cache.Deserialize(localHttpContext.Session.Get(CacheId));
            sessionLock.ExitReadLock();
        }

        private void BeforeAccessNotifications(TokenCacheNotificationArgs args)
        {
            Load();
        }

        private void AfterAccessNotifications(TokenCacheNotificationArgs args)
        {
            if (cache.HasStateChanged)
            {
                PersistCache();
            }
        }
    }
}
