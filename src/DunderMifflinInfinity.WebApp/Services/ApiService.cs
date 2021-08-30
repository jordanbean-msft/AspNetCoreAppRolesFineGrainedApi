#nullable enable

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Newtonsoft.Json;

namespace DunderMifflinInfinity.WebApp.Services
{
  public class ApiService<T>
  {
    private readonly IHttpContextAccessor contextAccessor;
    private readonly HttpClient httpClient;
    private readonly string scope = string.Empty;
    private readonly string apiBaseAddress = string.Empty;
    private readonly string apiEndpoint = string.Empty;
    private readonly ITokenAcquisition tokenAcquisition;

    private Func<T, int> editPropertyAccessor;

    public ApiService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor contextAccessor, string scope, string apiEndpoint, Func<T, int> editPropertyAccessor)
    {
      this.httpClient = httpClient;
      this.tokenAcquisition = tokenAcquisition;
      this.contextAccessor = contextAccessor;
      this.scope = scope;
      this.apiEndpoint = apiEndpoint;
      this.editPropertyAccessor = editPropertyAccessor;
      apiBaseAddress = configuration["DunderMifflinInfinity.Api:ApiBaseAddress"];
    }

    private async Task PrepareAuthenticatedClient()
    {
      var accessToken = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
      httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    private async Task<HttpResponseMessage> ExecuteApi(HttpMethod httpMethod, string apiEndpoint, StringContent? jsonContent = null)
    {
      await PrepareAuthenticatedClient();
      HttpRequestMessage httpRequestMessage = new HttpRequestMessage(httpMethod, apiBaseAddress + apiEndpoint){
        Content = jsonContent
      };

      return await httpClient.SendAsync(httpRequestMessage);
    }

    public async Task<T> AddAsync(T obj)
        {
            var jsonRequest = JsonConvert.SerializeObject(obj);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await ExecuteApi(HttpMethod.Post, $"/api/{apiEndpoint}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<T>(content);

                return obj;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task DeleteAsync(int id)
        {
            var response = await ExecuteApi(HttpMethod.Delete, $"/api/${apiEndpoint}/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<T> EditAsync(T obj)
        {
            var jsonRequest = JsonConvert.SerializeObject(obj);
            var jsoncontent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            var response = await ExecuteApi(HttpMethod.Put, $"/api/{apiEndpoint}/{editPropertyAccessor(obj)}", jsoncontent);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                obj = JsonConvert.DeserializeObject<T>(content);

                return obj;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            var response = await ExecuteApi(HttpMethod.Get, $"/api/{apiEndpoint}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                IEnumerable<T> sale = JsonConvert.DeserializeObject<IEnumerable<T>>(content);

                return sale;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }

        public async Task<T> GetAsync(int id)
        {
            var response = await ExecuteApi(HttpMethod.Get, $"/api/{apiEndpoint}/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                T sale = JsonConvert.DeserializeObject<T>(content);

                return sale;
            }

            throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
        }
  }
}