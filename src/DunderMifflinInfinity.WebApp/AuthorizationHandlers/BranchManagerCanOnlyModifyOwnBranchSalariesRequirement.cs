using Microsoft.AspNetCore.Authorization;

namespace DunderMifflinInfinity.WebApp.AuthorizationHandlers
{
    public class BranchManagerCanOnlyModifyOwnBranchSalariesRequirement : IAuthorizationRequirement
    {
    }
}