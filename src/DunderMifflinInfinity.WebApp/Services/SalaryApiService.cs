using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DunderMifflinInfinity.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

namespace DunderMifflinInfinity.WebApp.Services {
  public class SalaryApiService : ApiService<Salary>, ISalaryApiService
  {
        public SalaryApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor) : base(tokenAcquisition, httpClient, configuration, contextAccessor, configuration["DunderMifflinInfinity.Api:SalaryScope"], "salaries", (Salary s) => { return s.SalaryID; })
        {
        }
  }
}