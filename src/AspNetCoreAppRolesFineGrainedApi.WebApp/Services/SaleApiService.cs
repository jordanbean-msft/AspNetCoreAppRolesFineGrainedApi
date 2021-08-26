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
  public class SaleApiService : ISaleApiService
  {
      private readonly IHttpContextAccessor contextAccessor;
        private readonly HttpClient httpClient;
        private readonly string saleListScope = string.Empty;
        private readonly string apiBaseAddress = string.Empty;
        private readonly ITokenAcquisition tokenAcquisition;

        public SaleApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.httpClient = httpClient;
            this.tokenAcquisition = tokenAcquisition;
            this.contextAccessor = contextAccessor;
            saleListScope = configuration["AspNetCoreAppRolesFineGrainedApi.Api:SaleScope"];
            apiBaseAddress = configuration["AspNetCoreAppRolesFineGrainedApi.Api:ApiBaseAddress"];
        }

        public async Task<Sale> AddAsync(Sale sale)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(sale);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await this.httpClient.PostAsync($"{ apiBaseAddress}/api/sale", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                sale = JsonConvert.DeserializeObject<Sale>(content);

                return sale;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(int id)
        {
            await PrepareAuthenticatedClient();

            var response = await httpClient.DeleteAsync($"{ apiBaseAddress}/api/sale/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<Sale> EditAsync(Sale sale)
        {
            await PrepareAuthenticatedClient();

            var jsonRequest = JsonConvert.SerializeObject(sale);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json-patch+json");
            var response = await httpClient.PatchAsync($"{ apiBaseAddress}/api/sale/{sale.SaleID}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                sale = JsonConvert.DeserializeObject<Sale>(content);

                return sale;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<Sale>> GetAsync()
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/sale");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<Sale> sale = JsonConvert.DeserializeObject<IEnumerable<Sale>>(content);

                return sale;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        private async Task PrepareAuthenticatedClient()
        {
            var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { saleListScope });
            Debug.WriteLine($"access token-{accessToken}");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<Sale> GetAsync(int id)
        {
            await PrepareAuthenticatedClient();
            var response = await httpClient.GetAsync($"{ apiBaseAddress}/api/sale/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                Sale sale = JsonConvert.DeserializeObject<Sale>(content);

                return sale;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
  }
}