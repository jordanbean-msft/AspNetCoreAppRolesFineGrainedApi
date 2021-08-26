using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;

namespace AspNetCoreAppRolesFineGrainedApi.WebApp.Services
{
  public interface IEmployeeApiService
  {
    Task<IEnumerable<Employee>> GetAsync();

    Task<Employee> GetAsync(int id);

    Task DeleteAsync(int id);

    Task<Employee> AddAsync(Employee employee);

    Task<Employee> EditAsync(Employee employee);
  }
}