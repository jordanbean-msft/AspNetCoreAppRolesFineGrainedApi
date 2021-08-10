using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreWithAppRolesAndFineGrained
{
    public class BranchManagerCanOnlyModifyOwnBranchSalaries : IAuthorizationRequirement
    {
    }
}