using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreWithAppRoleAndFineGrained;
using AspNetCoreWithAppRoleAndFineGrained.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace AspNetCoreWithAppRolesAndFineGrained.AuthorizationHandlers {
    // public class SalaryRegionalManagerAuthorizationCrudHandler : AuthorizationHandler<OperationAuthorizationRequirement, Salary> {
    //   // protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
    //   //                                                OperationAuthorizationRequirement requirement,
    //   //                                                Salary salary) {
    //   //   if(context.User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Any(x => x.Value == AppRoles.RegionalManagerReadWrite.Name)
    //   //     ) {
    //   //       context.Succeed(requirement);
    //   //   }

    //   //   return Task.CompletedTask;
    //   // }
    // }
}