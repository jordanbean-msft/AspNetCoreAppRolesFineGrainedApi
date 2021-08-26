using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreAppRolesFineGrainedApi
{
    public class CannotModifyOwnSalaryRequirement : IAuthorizationRequirement
    {
    }
}