
namespace Demoulas.ProfitSharing.Services.Reports.Breakdown;

// https://bitbucket.org/demoulas/hpux/src/master/copy/WS-DETERMINE-STORE-TYPE.cpy

internal static class StoreTypes
{
    public static readonly HashSet<int> RetailStores = Enumerable.Range(1, 140).ToHashSet();

    public const int MbdWarehouse = 901;
    public const int GroceryWarehouse = 990;
    public const int PoultryWarehouse = 991;
    public const int MeatWarehouse = 992;
    public const int ProduceWarehouse = 993;
    public const int BakeryWarehouse = 994;
    public const int DairyWarehouse = 995;
    public const int SeafoodWarehouse = 996;
    public const int LawrenceWarehouse = 998;

    public static readonly HashSet<int> WarehouseAll = new()
    {
        MbdWarehouse, GroceryWarehouse, PoultryWarehouse, MeatWarehouse,
        ProduceWarehouse, BakeryWarehouse, DairyWarehouse, SeafoodWarehouse, LawrenceWarehouse
    };

    public static readonly HashSet<int> WarehousePay = new()
    {
        GroceryWarehouse, PoultryWarehouse, MeatWarehouse, ProduceWarehouse,
        BakeryWarehouse, DairyWarehouse, SeafoodWarehouse, LawrenceWarehouse
    };

    public const int Drivers = 985;

    public const int Shopify = 984;
    public const int IndianRidge = 986;
    public const int ExcelRefrig = 987;
    public const int DataProcessing = 988;
    public const int Advertising = 989;
    public const int Garage = 997;
    public const int OfficeExpense = 999;

    public static readonly HashSet<int> Headquarters = new()
    {
        Shopify, IndianRidge, ExcelRefrig, DataProcessing, Advertising, Garage, OfficeExpense
    };

    public static readonly HashSet<int> HeadquartersNoRefrig = new()
    {
        Shopify, IndianRidge, DataProcessing, Advertising, Garage, OfficeExpense
    };

    public const int PsPensionRetired = 700;
    public const int PsPensionActive = 701;
    public const int PsTerminatedEmployees = 800;
    public const int PsTerminatedEmployeesZero = 801;
    public const int PsTerminatedEmployeesNoVest = 802;
    public const int PsMonthlyEmployees = 900;

    public static readonly HashSet<int> ProfitSharingReport = new()
    {
        PsPensionRetired, PsPensionActive, PsTerminatedEmployees,
        PsTerminatedEmployeesZero, PsMonthlyEmployees
    };

    public const int LeeDrug = 903;
    public const int Corporate = 900;
}
