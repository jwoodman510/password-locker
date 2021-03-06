﻿using System.Linq;
using Ksu.Caching;
using Ksu.DataAccess.Dal;
using Ksu.Model;

namespace Ksu.PasswordLocker.Identity
{
    public interface IUserCache
    {
        CachedUser Get(string id);
        string GetRoleId(string userId);
        void Remove(string id);
    }

    public class UserCache : IUserCache
    {
        private readonly ICache _cache;
        private readonly IUserDal _userDal;

        public UserCache(ICache cache, IUserDal userDal)
        {
            _cache = cache;
            _userDal = userDal;
        }

        public CachedUser Get(string id)
        {
            return !string.IsNullOrWhiteSpace(id)
                ? GetImpl(id)
                : null;
        }

        public string GetRoleId(string userId)
        {
            return !string.IsNullOrWhiteSpace(userId)
                ? GetImpl(userId)?.RoleId
                : null;
        }

        public void Remove(string id)
        {
            if(!string.IsNullOrWhiteSpace(id))
                _cache.Delete(id);
        }

        private CachedUser GetImpl(string id)
        {
            var user = _cache.GetValue<CachedUser>(id);

            if (user != null)
                return user;

            var dbUser = _userDal.Get(id);
            user = new CachedUser
            {
                RoleId = dbUser?.AspNetRoles?.FirstOrDefault()?.Id,
                CompanyId = dbUser?.Companies?.FirstOrDefault()?.CompanyId ?? 0,
                CompanyName = dbUser?.Companies?.FirstOrDefault()?.CompanyName
            };

            _cache.SetValue(id, user, 60 * 5);

            return user;
        }
    }
}