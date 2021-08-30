using Xunit;
using DunderMifflinInfinity.Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using DunderMifflinInfinity.Api.Models;
using DunderMifflinInfinity.Api.Data;
using System.Collections.Generic;

namespace DunderMifflinInfinity.Api.Tests
{
  public class SalaryControllerIndexUnitTests : SalaryControllerUnitTests
  {
    [Fact]
    public async void TestAnonymousUserIndex()
    {
      using (var context = new DunderMifflinInfinityDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = anonymousUser }
        };

        var result = await salaryController.GetSalaries();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Salary>>>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(actionResult.Value);
        Assert.Empty(model);
      }
    }

    [Fact]
    public async void TestSalespersonUserIndex()
    {
      using (var context = new DunderMifflinInfinityDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = salespersonUser }
        };

        var result = await salaryController.GetSalaries();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Salary>>>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(actionResult.Value);

        Assert.Single(model);
        Assert.Equal("dwightschrute@jordanbeandemo.onmicrosoft.com", model.First().Employee.UserPrincipalName);
        Assert.Equal(60000, model.First().Value);
      }
    }

    [Fact]
    public async void TestRegionalManagerUserIndex()
    {
      using (var context = new DunderMifflinInfinityDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = regionalManagerUser }
        };

        var result = await salaryController.GetSalaries();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Salary>>>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(actionResult.Value);

        Assert.Equal(3, model.Count());
      }
    }

    [Fact]
    public async void TestCFOUserIndex()
    {
      using (var context = new DunderMifflinInfinityDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = cfoUser }
        };

        var result = await salaryController.GetSalaries();

        var actionResult = Assert.IsType<ActionResult<IEnumerable<Salary>>>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Salary>>(actionResult.Value);

        Assert.Equal(6, model.Count());
      }
    }
  }
}