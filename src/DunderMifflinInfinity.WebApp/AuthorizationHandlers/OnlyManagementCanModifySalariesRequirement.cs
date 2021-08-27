using Microsoft.AspNetCore.Authorization;

namespace DunderMifflinInfinity.WebApp.AuthorizationHandlers
{
  public class OnlyManagementCanModifySalariesRequirement : IAuthorizationRequirement
  {
  }   
}