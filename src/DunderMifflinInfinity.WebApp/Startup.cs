using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DunderMifflinInfinity.WebApp.AuthorizationHandlers;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using DunderMifflinInfinity.WebApp.Services;

namespace DunderMifflinInfinity.WebApp
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Environment = env;
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      var initialScopes = new string[] { "openId", "profile" };
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
      services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(options =>
        {
          Configuration.Bind("AzureAd", options);
          options.GetClaimsFromUserInfoEndpoint = true;
          options.TokenValidationParameters.RoleClaimType = "roles";
        }, options =>
        {
          Configuration.Bind("AzureAD", options);
        })
        .EnableTokenAcquisitionToCallDownstreamApi(options =>
        {
          Configuration.Bind("AzureAD", options);
        }, new string[] { Configuration["AspNetCoreAppRolesFineGrainedApi.Api:SalaryScope"]})
        .AddInMemoryTokenCaches();

      services.AddAuthorization(options =>
      {
        options.AddPolicy(Policies.GENERAL, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE, AppRoles.SALESPERSON_READWRITE, AppRoles.GENERAL_READWRITE));
        options.AddPolicy(Policies.MANAGEMENT, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE));
        options.AddPolicy(Policies.SALESPERSON, policy => policy.RequireRole(AppRoles.SALESPERSON_READWRITE));
        options.AddPolicy(Policies.SALARY, policy => {
          policy.Requirements.Add(new CannotModifyOwnSalaryRequirement());
          policy.Requirements.Add(new OnlyManagementCanModifySalariesRequirement());
          policy.Requirements.Add(new BranchManagerCanOnlyModifyOwnBranchSalariesRequirement());
        });
      });

      services.AddSingleton<IAuthorizationHandler, SalaryAuthorizationHandler>();

      services.AddHttpClient<IBranchApiService, BranchApiService>();
      services.AddHttpClient<IEmployeeApiService, EmployeeApiService>();
      services.AddHttpClient<ISalaryApiService, SalaryApiService>();
      services.AddHttpClient<ISaleApiService, SaleApiService>();

      services.AddControllersWithViews(options =>
      {
        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
      }).AddMicrosoftIdentityUI();

      services.AddRazorPages()
        .AddMicrosoftIdentityUI();      
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapRazorPages();
      });
    }
  }
}
