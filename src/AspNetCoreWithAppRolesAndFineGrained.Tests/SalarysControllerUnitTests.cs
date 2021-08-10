using System;
using Xunit;
using System.Security.Principal;
using System.Security.Claims;
using AspNetCoreWithAppRolesAndFineGrained.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AspNetCoreWithAppRolesAndFineGrained.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AspNetCoreWithAppRolesAndFineGrained.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreWithAppRolesAndFineGrained.AuthorizationHandlers;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace AspNetCoreWithAppRolesAndFineGrained.Tests
{
  public class SalaryControllerUnitTests
  {
    ClaimsPrincipal anonymousUser;
    ClaimsPrincipal salespersonUser;
    ClaimsPrincipal regionalManagerUser;
    ClaimsPrincipal cfoUser;
    SalariesController salaryController;

    DbConnection connection;
    DbContextOptions<AspNetCoreWithAppRolesAndFineGrainedDbContext> contextOptions;

    IAuthorizationService salaryAuthorizationService;

    public SalaryControllerUnitTests()
    {
      SetupUsers();

      contextOptions = new DbContextOptionsBuilder<AspNetCoreWithAppRolesAndFineGrainedDbContext>()
            .UseSqlite(CreateInMemoryDatabase())
            .Options;

      connection = RelationalOptionsExtension.Extract(contextOptions).Connection;

      SeedDatabase();

      SetupAuthorization();
    }

    [Fact]
    public async void TestAnonymousUserIndex()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = anonymousUser }
        };

        var result = await salaryController.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(
          viewResult.ViewData.Model);
        Assert.Empty(model);
      }
    }

    [Fact]
    public async void TestSalespersonUserIndex()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = salespersonUser }
        };

        var result = await salaryController.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(
          viewResult.ViewData.Model);

        Assert.Single(model);
        Assert.Equal("dwightschrute@jordanbeandemo.onmicrosoft.com", model.First().Employee.UserPrincipalName);
        Assert.Equal(60000, model.First().Value);
      }
    }

    [Fact]
    public async void TestRegionalManagerUserIndex()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = regionalManagerUser }
        };

        var result = await salaryController.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(
          viewResult.ViewData.Model);

        Assert.Equal(3, model.Count());
      }
    }

    [Fact]
    public async void TestCFOUserIndex()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = cfoUser }
        };

        var result = await salaryController.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(
          viewResult.ViewData.Model);

        Assert.Equal(6, model.Count());
      }
    }

    private void SeedDatabase()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        var branches = new Branch[] {
        new Branch { Name="Corporate", RegionalManagerAADGroupId="", SalespersonAADGroupId="" },
        new Branch { Name="Scranton", RegionalManagerAADGroupId="49c711e6-ad15-43d5-978e-4f56197994bd", SalespersonAADGroupId="4e45d8c9-f08e-498e-a320-62eea0ed53f5" },
        new Branch { Name="Stamford", RegionalManagerAADGroupId="f4022ed6-4120-494f-813c-ef078d3876f8", SalespersonAADGroupId="16b56a6c-d017-40ee-b75b-a53ff7f9aa19" }
      };

        foreach (Branch b in branches)
        {
          context.Branches.Add(b);
        }
        context.SaveChanges();

        var employees = new Employee[] {
        new Employee { FirstName="Michael",LastName="Scott",BranchID=2, UserPrincipalName="michaelscott@jordanbeandemo.onmicrosoft.com"},
        new Employee { FirstName="Josh",LastName="Porter",BranchID=3, UserPrincipalName="joshporter@jordanbeandemo.onmicrosoft.com"},
        new Employee { FirstName="Dwight",LastName="Schrute",BranchID=2, UserPrincipalName="dwightschrute@jordanbeandemo.onmicrosoft.com"},
        new Employee { FirstName="Karen",LastName="Filippelli",BranchID=3, UserPrincipalName="karenfilippelli@jordanbeandemo.onmicrosoft.com"},
        new Employee { FirstName="Pam",LastName="Beasley",BranchID=2, UserPrincipalName="pambeasley@jordanbeandemo.onmicrosoft.com"},
        new Employee { FirstName="David",LastName="Wallace", BranchID=1, UserPrincipalName="davidwallace@jordanbeandemo.onmicrosoft.com"}
      };

        foreach (Employee e in employees)
        {
          context.Employees.Add(e);
        }
        context.SaveChanges();

        var salaries = new Salary[] {
        new Salary { EmployeeID=1, Value=60000 },
        new Salary { EmployeeID=2, Value=80000 },
        new Salary { EmployeeID=3, Value=50000 },
        new Salary { EmployeeID=4, Value=50000 },
        new Salary { EmployeeID=5, Value=30000 },
        new Salary { EmployeeID=6, Value=200000 }
      };

        foreach (Salary s in salaries)
        {
          context.Salaries.Add(s);
        }
        context.SaveChanges();

        var sales = new Sale[] {
        new Sale { EmployeeID=3, Value=1000 },
        new Sale { EmployeeID=3, Value=2000 },
        new Sale { EmployeeID=4, Value=500 },
        new Sale { EmployeeID=4, Value=600 },
      };

        foreach (Sale s in sales)
        {
          context.Sales.Add(s);
        }
        context.SaveChanges();
      }
    }

    private IAuthorizationService BuildAuthorizationService(Action<IServiceCollection> setupServices = null)
    {
      var services = new ServiceCollection();
      services.AddAuthorization();
      services.AddLogging();
      services.AddOptions();
      setupServices?.Invoke(services);
      return services.BuildServiceProvider().GetRequiredService<IAuthorizationService>();
    }

    private void SetupUsers()
    {
      anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
      salespersonUser = new ClaimsPrincipal(
                          new ClaimsIdentity(
                            new GenericIdentity("dwightschrute@jordanbeandemo.onmicrosoft.com"),
                            new Claim[]
                            {
                              new Claim("upn", "dwightschrute@jordanbeandemo.onmicrosoft.com"),
                              new Claim(ClaimTypes.Role, "Salesperson.ReadWrite"),
                              new Claim(ClaimTypes.Role, "General.ReadWrite"),
                              new Claim("groups", "4e45d8c9-f08e-498e-a320-62eea0ed53f5"),
                              new Claim("groups", "1b7e2975-47f2-40e0-b18d-e0a47d29859b")
                            }));
      regionalManagerUser = new ClaimsPrincipal(
                              new ClaimsIdentity(
                                new GenericIdentity("michaelscott@jordanbeandemo.onmicrosoft.com"),
                                new Claim[]
                                {
                                  new Claim("upn", "michaelscott@jordanbeandemo.onmicrosoft.com"),
                                  new Claim(ClaimTypes.Role, "RegionalManager.ReadWrite"),
                                  new Claim(ClaimTypes.Role, "General.ReadWrite"),
                                  new Claim("groups", "49c711e6-ad15-43d5-978e-4f56197994bd"),
                                  new Claim("groups", "1b7e2975-47f2-40e0-b18d-e0a47d29859b")
                                }));
      cfoUser = new ClaimsPrincipal(
                  new ClaimsIdentity(
                    new GenericIdentity("davidwallace@jordanbeandemo.onmicrosoft.com"),
                    new Claim[]
                    {
                      new Claim("upn", "davidwallace@jordanbeandemo.onmicrosoft.com"),
                      new Claim(ClaimTypes.Role, "CFO.ReadWrite"),
                      new Claim(ClaimTypes.Role, "General.ReadWrite"),
                      new Claim("groups", "fc0f2d1e-b4ba-4a43-b6cf-4e6c9dc06138"),
                      new Claim("groups", "1b7e2975-47f2-40e0-b18d-e0a47d29859b")
                    }));
    }

    public void Dispose()
    {
      connection.Dispose();
    }

    private static DbConnection CreateInMemoryDatabase()
    {
      var connection = new SqliteConnection("Filename=:memory:");

      connection.Open();

      return connection;
    }

    private void SetupAuthorization()
    {
      salaryAuthorizationService = BuildAuthorizationService(services =>
      {
        services.AddScoped<IAuthorizationHandler, SalaryAuthorizationHandler>();

        services.AddAuthorization(options =>
          {
            options.AddPolicy(Policies.GENERAL, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE, AppRoles.SALESPERSON_READWRITE, AppRoles.GENERAL_READWRITE));
            options.AddPolicy(Policies.MANAGEMENT, policy => policy.RequireRole(AppRoles.CFO_READWRITE, AppRoles.REGIONAL_MANAGER_READWRITE));
            options.AddPolicy(Policies.SALESPERSON, policy => policy.RequireRole(AppRoles.SALESPERSON_READWRITE));
            options.AddPolicy(Policies.SALARY, policy =>
            {
              policy.Requirements.Add(new CannotModifyOwnSalaryRequirement());
              policy.Requirements.Add(new OnlyManagementCanModifySalariesRequirement());
            });
          });
      });
    }
  }
}
