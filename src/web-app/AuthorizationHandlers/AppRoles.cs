using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AspNetCoreWithAppRoleAndFineGrained
{
  public static class AppRoles
  {
        public const string REGIONAL_MANAGER_READWRITE = "RegionalManager.ReadWrite";
        public const string SALESPERSON_READWRITE = "Salesperson.ReadWrite";
        public const string CFO_READWRITE = "CFO.ReadWrite";
  }
}