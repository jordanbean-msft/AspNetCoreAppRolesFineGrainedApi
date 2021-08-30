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
  public class BranchApiService : ApiService<Branch>, IBranchApiService
  {
        public BranchApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor) : base(tokenAcquisition, httpClient, configuration, contextAccessor, configuration["DunderMifflinInfinity.Api:DefaultScope"], "branches", (Branch b) => { return b.BranchID; })
        {
        }
  }
}