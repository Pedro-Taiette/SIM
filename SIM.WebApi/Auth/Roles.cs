namespace SIM.WebApi.Auth;

/// <summary>
/// Role constants for use in [Authorize(Roles = ...)] attributes.
/// Values must match exactly the UserRole enum names.
/// </summary>
public static class Roles
{
    public const string Admin            = "Admin";
    public const string Pharmacist       = "Pharmacist";
    public const string StockManager     = "StockManager";
    public const string ReceivingOperator = "ReceivingOperator";

    public const string AdminOrStockManager =
        Admin + "," + StockManager;

    public const string AllRoles =
        Admin + "," + Pharmacist + "," + StockManager + "," + ReceivingOperator;
}
