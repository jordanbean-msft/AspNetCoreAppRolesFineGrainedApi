using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;

namespace AspNetCoreAppRolesFineGrainedApi.WebApp.Services
{
  public interface ISalaryApiService
  {
    Task<IEnumerable<Salary>> GetAsync();

    Task<Salary> GetAsync(int id);

    Task DeleteAsync(int id);

    Task<Salary> AddAsync(Salary salary);

    Task<Salary> EditAsync(Salary salary);
  }
}