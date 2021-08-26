using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

namespace AspNetCoreAppRolesFineGrainedApi.WebApp.Services {
  public class BranchApiService : IBranchApiService
  {
      private readonly IHttpContextAccessor contextAccessor;
        private readonly HttpClient httpClient;
        private readonly string branchListScope = string.Empty;
        private readonly string apiBaseAddress = string.Empty;
        private readonly ITokenAcquisition tokenAcquisition;

        public BranchApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.httpClient = httpClient;
            this.tokenAcquisition = tokenAcquisition;
            this.contextAccessor = contextAccessor;
            branchListScope = configuration["AspNetCoreAppRolesFineGrainedApi.Api:DefaultScope"];
            apiBaseAddress = configuration["AspNetCoreAppRolesFineGrainedApi.Api:ApiBaseAddress"];
        }

        public async Task<Branch> AddAsync(Branch branch)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(branch);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PostAsync($"{ apiBaseAddress}/api/branch", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                branch = JsonConvert.DeserializeObject<Branch>(content);

                return branch;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(int id)
        {
            await PrepareAuthenticatedClient();

            var response = await httpClient.DeleteAsync($"{ apiBaseAddress}/api/branch/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Branch> EditAsync(Branch branch)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(branch);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
            var response = await httpClient.PatchAsync($"{ apiBaseAddress}/api/branch/{branch.BranchID}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                branch = JsonConvert.DeserializeObject<Branch>(content);

                return branch;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<Branch>> GetAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/branch");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Branch> branch = JsonConvert.DeserializeObject<IEnumerable<Branch>>(content);

                return branch;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { branchListScope });
            Debug.WriteLine($"access token-{accessToken}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Branch> GetAsync(int id)
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/branch/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Branch branch = JsonConvert.DeserializeObject<Branch>(content);

                return branch;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
  }
}