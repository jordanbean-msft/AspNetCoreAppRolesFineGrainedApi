using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;

namespace AspNetCoreAppRolesFineGrainedApi.WebApp.Services
{
  public interface ISaleApiService
  {
    Task<IEnumerable<Sale>> GetAsync();

    Task<Sale> GetAsync(int id);

    Task DeleteAsync(int id);

    Task<Sale> AddAsync(Sale sale);

    Task<Sale> EditAsync(Sale sale);
  }
}