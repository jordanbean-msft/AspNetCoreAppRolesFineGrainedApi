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
  public class SalaryApiService : ISalaryApiService
  {
      private readonly IHttpContextAccessor contextAccessor;
        private readonly HttpClient httpClient;
        private readonly string salaryListScope = string.Empty;
        private readonly string apiBaseAddress = string.Empty;
        private readonly ITokenAcquisition tokenAcquisition;

        public SalaryApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.httpClient = httpClient;
            this.tokenAcquisition = tokenAcquisition;
            this.contextAccessor = contextAccessor;
            salaryListScope = configuration["AspNetCoreAppRolesFineGrainedApi.Api:SalaryScope"];
            apiBaseAddress = configuration["AspNetCoreAppRolesFineGrainedApi.Api:ApiBaseAddress"];
        }

        public async Task<Salary> AddAsync(Salary salary)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(salary);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PostAsync($"{ apiBaseAddress}/api/salary", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                salary = JsonConvert.DeserializeObject<Salary>(content);

                return salary;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(int id)
        {
            await PrepareAuthenticatedClient();

            var response = await httpClient.DeleteAsync($"{ apiBaseAddress}/api/salary/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Salary> EditAsync(Salary salary)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(salary);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
            var response = await httpClient.PatchAsync($"{ apiBaseAddress}/api/salary/{salary.SalaryID}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                salary = JsonConvert.DeserializeObject<Salary>(content);

                return salary;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<Salary>> GetAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/salary");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Salary> salary = JsonConvert.DeserializeObject<IEnumerable<Salary>>(content);

                return salary;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { salaryListScope });
            Debug.WriteLine($"access token-{accessToken}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Salary> GetAsync(int id)
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/salary/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Salary salary = JsonConvert.DeserializeObject<Salary>(content);

                return salary;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
  }
}