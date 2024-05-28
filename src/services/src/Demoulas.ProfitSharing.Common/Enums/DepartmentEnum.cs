namespace Demoulas.ProfitSharing.Common.Enums;

/*
 *The PY_DP column indicates department to which employee is assigned.
   1   -   grocery
   2   -   meat
   3   -   produce
   4   -   deli
   5   -   dairy
   6   -   beer/wine
   7   -   bakery
 */

public enum DepartmentEnum : byte
{
    Grocery = 1,
    Meat = 2,
    Produce = 3,
    Deli = 4,
    Dairy = 5,
    Beer_And_Wine = 6,
    Bakery = 7,
}
