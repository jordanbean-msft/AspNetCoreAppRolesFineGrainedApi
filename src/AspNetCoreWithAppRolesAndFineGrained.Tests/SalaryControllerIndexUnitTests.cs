using Xunit;
using AspNetCoreWithAppRolesAndFineGrained.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AspNetCoreWithAppRolesAndFineGrained.Data;
using System.Linq;
using AspNetCoreWithAppRolesAndFineGrained.Models;
using System.Collections.Generic;

namespace AspNetCoreWithAppRolesAndFineGrained.Tests
{
  public class SalaryControllerIndexUnitTests : SalaryControllerUnitTests
  {
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
  }
}