using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWithAppRoleAndFineGrained;
using AspNetCoreWithAppRoleAndFineGrained.AuthorizationHandlers;
using AspNetCoreWithAppRoleAndFineGrained.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AspNetCoreWithAppRolesAndFineGrained.AuthorizationHandlers {
    public class SalaryAuthorizationCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, Salary> {
      protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                     OperationAuthorizationRequirement requirement,
                                                     Salary salary) {
        if(context.User.Identity.Name == salary.Employee.UserPrincipalName &&
           requirement.Name == Operations.Read.Name) {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
      }
    }
}