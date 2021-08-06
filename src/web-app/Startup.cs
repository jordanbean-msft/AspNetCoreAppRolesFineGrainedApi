using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AspNetCoreWithAppRoleAndFineGrained.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using AspNetCoreWithAppRolesAndFineGrained.AuthorizationHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace AspNetCoreWithAppRoleAndFineGrained
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
        }, initialScopes)
        .AddInMemoryTokenCaches();

      services.AddDbContext<AspNetCoreWithAppRoleAndFineGrainedDbContext>(options =>
      {
        var connectionString = Configuration.GetConnectionString("sqlDatabaseConnectionString");

        if (Environment.IsDevelopment())
        {
          options.UseLazyLoadingProxies().UseSqlite(connectionString);
        }
        else
        {
          options.UseLazyLoadingProxies().UseSqlServer(connectionString);
        }
      });

      // services.AddDefaultIdentity<IdentityUser>()
      //   .AddRoles<IdentityUser>()
      //   .AddEntityFrameworkStores<AspNetCoreWithAppRoleAndFineGrainedDbContext>();

      services.AddDatabaseDeveloperPageExceptionFilter();

      services.AddControllersWithViews(options =>
      {
        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
        options.Filters.Add(new AuthorizeFilter(policy));
      });

      services.AddRazorPages()
        .AddMicrosoftIdentityUI();

       services.AddAuthorization(options =>
       {
         options.AddPolicy(Policies.General, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE, AppRoles.SALESPERSON_READWRITE));
         options.AddPolicy(Policies.Management, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE));
         options.AddPolicy(Policies.Salesperson, policy => policy.RequireRole(AppRoles.SALESPERSON_READWRITE));
       });
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
