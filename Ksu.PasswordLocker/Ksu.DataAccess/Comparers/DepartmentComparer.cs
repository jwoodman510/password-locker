using System.Collections.Generic;

namespace Ksu.DataAccess.Comparers
{
    public class DepartmentComparer : IEqualityComparer<Department>
    {
        private readonly CompareSetting _setting;

        public DepartmentComparer(CompareSetting setting)
        {
            _setting = setting;
        }

        public bool Equals(Department first, Department second)
        {
            if (_setting == CompareSetting.CompareProperties)
            {
                return first.DepartmentId == second.DepartmentId
                       && first.DepartmentName == second.DepartmentName;
            }

            if (_setting == CompareSetting.CompareObjects)
            {
                return first.Equals(second);
            }

            return first.DepartmentId == second.DepartmentId;
        }

        public int GetHashCode(Department u)
        {
            return _setting == CompareSetting.CompareIds
                ? u.DepartmentId.GetHashCode()
                : u.GetHashCode();
        }
    }
}
