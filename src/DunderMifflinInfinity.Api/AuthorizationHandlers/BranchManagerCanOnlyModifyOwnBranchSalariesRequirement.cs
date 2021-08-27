using Microsoft.AspNetCore.Authorization;

namespace DunderMifflinInfinity.Api.AuthorizationHandlers
{
    public class BranchManagerCanOnlyModifyOwnBranchSalariesRequirement : IAuthorizationRequirement
    {
    }
}