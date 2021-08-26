using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using AspNetCoreAppRolesFineGrainedApi.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreAppRolesFineGrainedApi.Api.AuthorizationHandlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace AspNetCoreAppRolesFineGrainedApi.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
      Configuration = configuration;
      environment = env;
    }

    public IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment environment;
    private readonly string ALLOW_SPECIFIC_ORIGINS = "ALLOW_SPECIFIC_ORIGINS";

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddMicrosoftIdentityWebApi(options =>
      {
        Configuration.Bind("AzureAD", options);
        options.TokenValidationParameters.NameClaimType = "roles";
      },
      options =>
      {
        Configuration.Bind("AzureAD", options);
      });
      services.AddDbContext<AspNetCoreAppRolesFineGrainedApiDbContext>(options =>
        {
          var connectionString = Configuration.GetConnectionString("sqlDatabaseConnectionString");

          if (environment.IsDevelopment())
          {
            options.UseLazyLoadingProxies().UseSqlite(connectionString);
          }
          else
          {
            options.UseLazyLoadingProxies().UseSqlServer(connectionString);
          }
        });
      services.AddCors(options => {
        options.AddPolicy(ALLOW_SPECIFIC_ORIGINS, builder => {
          builder.WithOrigins("https://localhost:5011");
        });
      });

      services.AddDatabaseDeveloperPageExceptionFilter();
      services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreAppRolesFineGrainedApi.Api", Version = "v1" });
      });
      services.AddAuthorization(options =>
      {
        options.AddPolicy(Policies.GENERAL, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE, AppRoles.SALESPERSON_READWRITE, AppRoles.GENERAL_READWRITE));
        options.AddPolicy(Policies.MANAGEMENT, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE));
        options.AddPolicy(Policies.SALESPERSON, policy => policy.RequireRole(AppRoles.SALESPERSON_READWRITE));
        options.AddPolicy(Policies.SALARY, policy =>
        {
          policy.Requirements.Add(new CannotModifyOwnSalaryRequirement());
          policy.Requirements.Add(new OnlyManagementCanModifySalariesRequirement());
          policy.Requirements.Add(new BranchManagerCanOnlyModifyOwnBranchSalariesRequirement());
        });
      });

      services.AddSingleton<IAuthorizationHandler, SalaryAuthorizationHandler>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCoreAppRolesFineGrainedApi.Api v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();
      app.UseCors(ALLOW_SPECIFIC_ORIGINS);
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
