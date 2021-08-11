using Xunit;
using AspNetCoreWithAppRolesAndFineGrained.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AspNetCoreWithAppRolesAndFineGrained.Data;
using System.Linq;
using AspNetCoreWithAppRolesAndFineGrained.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreWithAppRolesAndFineGrained.Tests
{
  public class SalaryControllerEditUnitTests : SalaryControllerUnitTests
  {
    [Fact]
    public async void TestAnonymousUserEdit()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = anonymousUser }
        };

        var result = await salaryController.Edit(1, new Salary() { SalaryID = 1, EmployeeID = 1, Value = 100000 });

        Assert.IsType<ForbidResult>(result);
      }
    }

    [Fact]
    public async void TestSalespersonUserEdit()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = salespersonUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "karenfilippelli@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        int previousSalary = beforeOwnBranchEmployee.Salary.Value;

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = 100000 });

        Assert.IsType<ForbidResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "karenfilippelli@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(previousSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }

    [Fact]
    public async void TestSalespersonUserEditThemselves()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = salespersonUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == salespersonUser.Identity.Name).First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        int previousSalary = beforeOwnBranchEmployee.Salary.Value;

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = 100000 });

        Assert.IsType<ForbidResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == salespersonUser.Identity.Name).First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(previousSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }

    [Fact]
    public async void TestRegionalManagerUserEditOwnBranchEmployee()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = regionalManagerUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "dwightschrute@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        const int newSalary = 100000;

        Assert.NotEqual(newSalary, beforeOwnBranchEmployee.Salary.Value);

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = newSalary });

        Assert.IsType<RedirectToActionResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "dwightschrute@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(newSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }

    [Fact]
    public async void TestRegionalManagerUserEditNonOwnBranchEmployee()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = regionalManagerUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "karenfilippelli@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        int previousSalary = beforeOwnBranchEmployee.Salary.Value;

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = 100000 });

        Assert.IsType<ForbidResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "karenfilippelli@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(previousSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }

    [Fact]
    public async void TestRegionalManagerUserEditOwnSalary()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = regionalManagerUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == regionalManagerUser.Identity.Name).First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        int previousSalary = beforeOwnBranchEmployee.Salary.Value;

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = 100000 });

        Assert.IsType<ForbidResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == regionalManagerUser.Identity.Name).First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(previousSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }

    [Fact]
    public async void TestCFOUserEdit()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = cfoUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "dwightschrute@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        const int newSalary = 100000;

        Assert.NotEqual(newSalary, beforeOwnBranchEmployee.Salary.Value);

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = newSalary });

        Assert.IsType<RedirectToActionResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == "dwightschrute@jordanbeandemo.onmicrosoft.com").First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(newSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }

    [Fact]
    public async void TestCFOUserEditOwnSalary()
    {
      using (var context = new AspNetCoreWithAppRolesAndFineGrainedDbContext(contextOptions))
      {
        salaryController = new SalariesController(salaryAuthorizationService, context);
        salaryController.ControllerContext = new ControllerContext()
        {
          HttpContext = new DefaultHttpContext { User = cfoUser }
        };

        var beforeOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == cfoUser.Identity.Name).First();
        //need to load the Salary navigation property
        await context.Entry(beforeOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        int previousSalary = beforeOwnBranchEmployee.Salary.Value;

        foreach (var entity in context.ChangeTracker.Entries())
        {
          entity.State = EntityState.Detached;
        }

        var result = await salaryController.Edit(beforeOwnBranchEmployee.Salary.SalaryID, new Salary() { SalaryID = beforeOwnBranchEmployee.Salary.SalaryID, EmployeeID = beforeOwnBranchEmployee.EmployeeID, Value = 100000 });

        Assert.IsType<ForbidResult>(result);
        var afterOwnBranchEmployee = context.Employees.Where(employee => employee.UserPrincipalName == cfoUser.Identity.Name).First();
        //need to load the Salary navigation property
        await context.Entry(afterOwnBranchEmployee).Reference(employee => employee.Salary).LoadAsync();

        Assert.Equal(previousSalary, afterOwnBranchEmployee.Salary.Value);
      }
    }
  }
}