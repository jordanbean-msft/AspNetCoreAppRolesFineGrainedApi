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
  public class EmployeeApiService : IEmployeeApiService
  {
      private readonly IHttpContextAccessor contextAccessor;
        private readonly HttpClient httpClient;
        private readonly string employeeListScope = string.Empty;
        private readonly string apiBaseAddress = string.Empty;
        private readonly ITokenAcquisition tokenAcquisition;

        public EmployeeApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.httpClient = httpClient;
            this.tokenAcquisition = tokenAcquisition;
            this.contextAccessor = contextAccessor;
            employeeListScope = configuration["AspNetCoreAppRolesFineGrainedApi.Api:DefaultScope"];
            apiBaseAddress = configuration["AspNetCoreAppRolesFineGrainedApi.Api:ApiBaseAddress"];
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(employee);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PostAsync($"{ apiBaseAddress}/api/employee", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                employee = JsonConvert.DeserializeObject<Employee>(content);

                return employee;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(int id)
        {
            await PrepareAuthenticatedClient();

            var response = await httpClient.DeleteAsync($"{ apiBaseAddress}/api/employee/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Employee> EditAsync(Employee employee)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(employee);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
            var response = await httpClient.PatchAsync($"{ apiBaseAddress}/api/employee/{employee.EmployeeID}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                employee = JsonConvert.DeserializeObject<Employee>(content);

                return employee;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<Employee>> GetAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/employee");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Employee> employee = JsonConvert.DeserializeObject<IEnumerable<Employee>>(content);

                return employee;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { employeeListScope });
            Debug.WriteLine($"access token-{accessToken}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Employee> GetAsync(int id)
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/employee/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Employee employee = JsonConvert.DeserializeObject<Employee>(content);

                return employee;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
  }
}