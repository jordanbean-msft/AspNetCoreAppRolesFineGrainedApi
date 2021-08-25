using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAppRolesFineGrainedApi
{
  public class OnlyManagementCanModifySalariesRequirement : IAuthorizationRequirement
  {
  }   
}