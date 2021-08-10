using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreWithAppRolesAndFineGrained
{
    public class CannotModifyOwnSalaryRequirement : IAuthorizationRequirement
    {
    }
}