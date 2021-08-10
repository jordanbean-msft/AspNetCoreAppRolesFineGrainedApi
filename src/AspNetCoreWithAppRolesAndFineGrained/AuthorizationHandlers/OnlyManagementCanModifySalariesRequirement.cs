using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreWithAppRolesAndFineGrained
{
  public class OnlyManagementCanModifySalariesRequirement : IAuthorizationRequirement
  {
  }   
}