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

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
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

      services.AddDatabaseDeveloperPageExceptionFilter();
      services.AddControllers().AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "AspNetCoreAppRolesFineGrainedApi.Api", Version = "v1" });
      });
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

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}