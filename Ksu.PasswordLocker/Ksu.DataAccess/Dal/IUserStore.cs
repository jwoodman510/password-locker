using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Ksu.DataAccess.Dal
{
    public interface IUserStore :
        IUserStore<AspNetUser>,
        IUserLoginStore<AspNetUser, string>,
        IUserClaimStore<AspNetUser, string>,
        IUserRoleStore<AspNetUser, string>,
        IUserPasswordStore<AspNetUser, string>,
        IUserSecurityStampStore<AspNetUser, string>,
        IQueryableUserStore<AspNetUser, string>,
        IUserEmailStore<AspNetUser, string>,
        IUserPhoneNumberStore<AspNetUser, string>,
        IUserTwoFactorStore<AspNetUser, string>,
        IUserLockoutStore<AspNetUser, string>
    {
        
    }

    public class UserStore : IUserStore
    {
        private readonly IContextProvider _context;
        private readonly DbSet<AspNetUser> _dbEntitySet;

        public IQueryable<AspNetUser> Users => _dbEntitySet;

        public UserStore(IContextProvider context)
        {
            _context = context;
            _dbEntitySet = _context.Set<AspNetUser>();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public async Task CreateAsync(AspNetUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));


            _context.Users.Add(user);

            await SaveChanges().ConfigureAwait(false);
        }

        public async Task UpdateAsync(AspNetUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));


            _context.Entry(user).State = EntityState.Modified;

            await SaveChanges().ConfigureAwait(false);
        }

        public async Task DeleteAsync(AspNetUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _context.Users.Remove(user);
            await SaveChanges().ConfigureAwait(false);
        }

        public Task<AspNetUser> FindByIdAsync(string userId)
        {
            return GetUserAggregateAsync(u => u.Id.Equals(userId));
        }

        public Task<AspNetUser> FindByNameAsync(string userName)
        {
            return GetUserAggregateAsync(u => u.UserName.ToUpper() == userName.ToUpper());
        }

        public Task AddLoginAsync(AspNetUser user, UserLoginInfo login)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if (login == null)
                throw new ArgumentNullException(nameof(login));

            user.Logins.Add(new AspNetUserLogin
            {
                UserId = user.Id, ProviderKey = login.ProviderKey, LoginProvider = login.LoginProvider
            });

            return Task.FromResult(0);
        }

        public Task RemoveLoginAsync(AspNetUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var entry = user.Logins.SingleOrDefault(l => l.LoginProvider == provider && l.ProviderKey == key);
            if (entry != null)
            {
                user.Logins.Remove(entry);
            }
            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            IList<UserLoginInfo> result = new List<UserLoginInfo>();
            foreach (var l in user.Logins)
            {
                result.Add(new UserLoginInfo(l.LoginProvider, l.ProviderKey));
            }
            return Task.FromResult(result);
        }

        public async Task<AspNetUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            var provider = login.LoginProvider;
            var key = login.ProviderKey;
            var userLogin = await _context.Logins.FirstOrDefaultAsync(l => l.LoginProvider == provider && l.ProviderKey == key);
            if (userLogin != null)
            {
                return await GetUserAggregateAsync(u => u.Id.Equals(userLogin.UserId));
            }
            return null;
        }

        public Task<IList<Claim>> GetClaimsAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            IList<Claim> result = new List<Claim>();
            foreach (var c in user.Claims)
            {
                result.Add(new Claim(c.ClaimType, c.ClaimValue));
            }
            return Task.FromResult(result);
        }

        public Task RemoveClaimAsync(AspNetUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            var claims = user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type).ToList();
            foreach (var c in claims)
            {
                user.Claims.Remove(c);
            }
            // Note: these claims might not exist in the dbset
            var query = _context.Claims.Where(uc => uc.UserId.Equals(user.Id) && uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
            foreach (var c in query)
            {
                _context.Claims.Remove(c);
            }
            return Task.FromResult(0);
        }

        public Task AddClaimAsync(AspNetUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            user.Claims.Add(new AspNetUserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });
            return Task.FromResult(0);
        }

        public Task AddToRoleAsync(AspNetUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var role = _context.Roles.FirstOrDefault(r => r.Name == roleName);

            if(role == null)
                throw new ArgumentNullException(nameof(role));

            if (role.AspNetUsers.Any(u => u.Id == user.Id))
                return Task.FromResult(0);

            role.AspNetUsers = role.AspNetUsers ?? new List<AspNetUser>();
            role.AspNetUsers.Add(user);

            _context.SaveChanges();

            return Task.FromResult(0);
        }

        public Task RemoveFromRoleAsync(AspNetUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName");
            }
            var roleEntity = _context.Roles.SingleOrDefault(r => r.Name.ToUpper() == roleName.ToUpper());
            if (roleEntity != null)
            {
                var userRole = user.Roles.FirstOrDefault(r => roleEntity.Name.ToUpper() == roleName.ToUpper());
                if (userRole != null)
                {
                    user.Roles.Remove(userRole);
                    roleEntity.Users.Remove(userRole);
                }
            }
            return Task.FromResult(0);
        }

        public Task<IList<string>> GetRolesAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var query = from userRoles in user.Roles
                        join roles in _context.Roles
                        on userRoles.RoleId equals roles.Id
                        select roles.Name;
            return Task.FromResult<IList<string>>(query.ToList());
        }

        public Task<bool> IsInRoleAsync(AspNetUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("roleName");
            }
            var any = _context.Roles.Where(r => r.Name.ToUpper() == roleName.ToUpper()).Any(r => r.Users.Any(ur => ur.UserId.Equals(user.Id)));
            return Task.FromResult(any);
        }

        public Task SetPasswordHashAsync(AspNetUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(AspNetUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(AspNetUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(AspNetUser user, string email)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(AspNetUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<AspNetUser> FindByEmailAsync(string email)
        {
            return GetUserAggregateAsync(u => u.Email.ToUpper() == email.ToUpper());
        }

        public Task SetPhoneNumberAsync(AspNetUser user, string phoneNumber)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(AspNetUser user, bool confirmed)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(AspNetUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(AspNetUser user, DateTimeOffset lockoutEnd)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(AspNetUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(AspNetUser user, bool enabled)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        #region helpers

        private async Task SaveChanges()
        {
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        private Task<AspNetUser> GetUserAggregateAsync(Expression<Func<AspNetUser, bool>> filter)
        {
            return _context.Users
                .Include(u => u.AspNetRoles)
                .Include(u => u.AspNetUserClaims)
                .Include(u => u.AspNetUserLogins)
                .FirstOrDefaultAsync(filter);
        }

        #endregion
    }
}
