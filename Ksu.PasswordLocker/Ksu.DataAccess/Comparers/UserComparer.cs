using System.Collections.Generic;

namespace Ksu.DataAccess.Comparers
{
    public class UserComparer : IEqualityComparer<AspNetUser>
    {
        private readonly CompareSetting _setting;

        public UserComparer(CompareSetting setting)
        {
            _setting = setting;
        }

        public bool Equals(AspNetUser first, AspNetUser second)
        {
            if (_setting == CompareSetting.CompareProperties)
            {
                return first.Id == second.Id
                       && first.Email == second.Email
                       && first.UserName == second.UserName
                       && first.PasswordHash == second.PasswordHash;
            }

            if (_setting == CompareSetting.CompareObjects)
            {
                return first.Equals(second);
            }

            return first.Id == second.Id;
        }

        public int GetHashCode(AspNetUser u)
        {
            return _setting == CompareSetting.CompareIds
                ? u.Id.GetHashCode()
                : u.GetHashCode();
        }
    }
}
