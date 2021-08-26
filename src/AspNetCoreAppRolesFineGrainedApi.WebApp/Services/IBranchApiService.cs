using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;

namespace AspNetCoreAppRolesFineGrainedApi.WebApp.Services
{
  public interface IBranchApiService
  {
    Task<IEnumerable<Branch>> GetAsync();

    Task<Branch> GetAsync(int id);

    Task DeleteAsync(int id);

    Task<Branch> AddAsync(Branch branch);

    Task<Branch> EditAsync(Branch branch);
  }
}