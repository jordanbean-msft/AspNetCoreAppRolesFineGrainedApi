using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreWithAppRolesAndFineGrained
{
    public class BranchManagerCanOnlyModifyOwnBranchSalariesRequirement : IAuthorizationRequirement
    {
    }
}