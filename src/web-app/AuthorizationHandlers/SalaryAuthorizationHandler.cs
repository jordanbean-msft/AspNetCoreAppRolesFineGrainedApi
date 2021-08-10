using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWithAppRolesAndFineGrained.Models;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreWithAppRolesAndFineGrained.AuthorizationHandlers
{
  public class SalaryAuthorizationHandler : IAuthorizationHandler
  {
    private IDictionary<Type, Action<IAuthorizationRequirement, AuthorizationHandlerContext>> handlers = new Dictionary<Type, Action<IAuthorizationRequirement, AuthorizationHandlerContext>>(){
      {
        typeof(CannotModifyOwnSalaryRequirement), (requirement, context) => CannotModifyOwnSalaryRequirementHandler(requirement, context)
      },
      {
        typeof(OnlyManagementCanModifySalariesRequirement), (requirement, context) => OnlyManagementCanModifySalariesRequirementHandler(requirement, context)
      }
    };

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
      var pendingRequirements = context.PendingRequirements.ToList();

      foreach (var requirement in pendingRequirements)
      {
        handlers[requirement.GetType()](requirement, context);
      }

      return Task.CompletedTask;
    }

    private static void CannotModifyOwnSalaryRequirementHandler(IAuthorizationRequirement requirement, AuthorizationHandlerContext context)
    {
      if (context.Resource is Salary)
      {
        if (context.User.Identity.Name != ((Salary)context.Resource).Employee.UserPrincipalName)
        {
          context.Succeed(requirement);
        }
      }
      else
      {
        context.Succeed(requirement);
      }
    }

    private static void OnlyManagementCanModifySalariesRequirementHandler(IAuthorizationRequirement requirement, AuthorizationHandlerContext context)
    {
      if (context.User.IsInRole(AppRoles.CFO_READWRITE)
              || context.User.IsInRole(AppRoles.REGIONAL_MANAGER_READWRITE))
      {
        context.Succeed(requirement);
      }
    }
  }
}