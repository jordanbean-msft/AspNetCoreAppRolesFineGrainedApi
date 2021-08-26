using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAppRolesFineGrainedApi.Api
{
  public class OnlyManagementCanModifySalariesRequirement : IAuthorizationRequirement
  {
  }   
}