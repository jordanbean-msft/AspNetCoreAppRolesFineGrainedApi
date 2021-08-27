using System.Collections.Generic;
using System.Threading.Tasks;
using DunderMifflinInfinity.WebApp.Models;

namespace DunderMifflinInfinity.WebApp.Services
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