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
  public class SaleApiService : ApiService<Sale>, ISaleApiService
  {
        public SaleApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor) : base(tokenAcquisition, httpClient, configuration, contextAccessor, configuration["DunderMifflinInfinity.Api:SaleScope"], "sales", (Sale s) => { return s.SaleID; })
        {
        }
  }
}