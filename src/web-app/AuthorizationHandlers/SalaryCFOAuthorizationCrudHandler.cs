using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWithAppRoleAndFineGrained;
using AspNetCoreWithAppRoleAndFineGrained.AuthorizationHandlers;
using AspNetCoreWithAppRoleAndFineGrained.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AspNetCoreWithAppRolesAndFineGrained.AuthorizationHandlers {
    // public class SalaryCFOAuthorizationCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, Salary> {
    //   protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
    //                                                  OperationAuthorizationRequirement requirement,
    //                                                  Salary salary) {
    //     if(context.User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Any(x => x.Value == AppRoles.CFOReadWrite.Name) && 
    //       requirement.Name == Operations.Read.Name) {
    //         context.Succeed(requirement);
    //     }

    //     return Task.CompletedTask;
    //   }
    // }
}