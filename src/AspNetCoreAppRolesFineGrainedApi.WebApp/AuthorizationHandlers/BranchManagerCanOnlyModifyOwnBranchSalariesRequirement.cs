using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAppRolesFineGrainedApi
{
    public class BranchManagerCanOnlyModifyOwnBranchSalariesRequirement : IAuthorizationRequirement
    {
    }
}