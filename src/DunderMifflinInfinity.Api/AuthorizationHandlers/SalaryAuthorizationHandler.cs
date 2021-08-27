using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DunderMifflinInfinity.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace DunderMifflinInfinity.Api.AuthorizationHandlers
{
  public class SalaryAuthorizationHandler : IAuthorizationHandler
  {
    private IDictionary<Type, Action<IAuthorizationRequirement, AuthorizationHandlerContext>> handlers = new Dictionary<Type, Action<IAuthorizationRequirement, AuthorizationHandlerContext>>(){
      {
        typeof(CannotModifyOwnSalaryRequirement), (requirement, context) => CannotModifyOwnSalaryRequirementHandler(requirement, context)
      },
      {
        typeof(OnlyManagementCanModifySalariesRequirement), (requirement, context) => OnlyManagementCanModifySalariesRequirementHandler(requirement, context)
      },
      {
        typeof(BranchManagerCanOnlyModifyOwnBranchSalariesRequirement), (requirement, context) => BranchManagerCanOnlyModifyOwnBranchSalariesRequirementHandler(requirement, context)
      }
    };

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
      var pendingRequirements = context.PendingRequirements.ToList();

      foreach (var requirement in pendingRequirements)
      {
        Action<IAuthorizationRequirement, AuthorizationHandlerContext> handler;
        if (handlers.TryGetValue(requirement.GetType(), out handler))
        {
          handler(requirement, context);
        }
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

    private static void BranchManagerCanOnlyModifyOwnBranchSalariesRequirementHandler(IAuthorizationRequirement requirement, AuthorizationHandlerContext context)
    {
      if (context.Resource is Salary && context.User.IsInRole(AppRoles.REGIONAL_MANAGER_READWRITE))
      {
        var aadGroups = context.User.Claims.Where(claim => claim.Type == "groups").Select(group => group.Value).ToList<string>();
      
        if (aadGroups.Contains(((Salary)context.Resource).Employee.Branch.RegionalManagerAADGroupId))
        {
          context.Succeed(requirement);
        }
      }
      else
      {
        context.Succeed(requirement);
      }      
    }
  }
}