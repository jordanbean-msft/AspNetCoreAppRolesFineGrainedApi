using System.Collections.Generic;
using System.Threading.Tasks;
using DunderMifflinInfinity.WebApp.Models;

namespace DunderMifflinInfinity.WebApp.Services
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