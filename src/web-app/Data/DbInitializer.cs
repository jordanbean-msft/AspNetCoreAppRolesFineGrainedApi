using AspNetCoreWithAppRolesAndFineGrained.Models;
using System;
using System.Linq;

namespace AspNetCoreWithAppRolesAndFineGrained.Data {
  public static class DbInitializer {
    public static void Initialize(AspNetCoreWithAppRolesAndFineGrainedDbContext context) {
      context.Database.EnsureCreated();

      if(context.Branches.Any()) {
        return; // DB has been seeded
      }

      var branches = new Branch[] {
        new Branch { Name="Corporate", RegionalManagerAADGroupId="", SalespersonAADGroupId="" },
        new Branch { Name="Scranton", RegionalManagerAADGroupId="49c711e6-ad15-43d5-978e-4f56197994bd", SalespersonAADGroupId="4e45d8c9-f08e-498e-a320-62eea0ed53f5" },
        new Branch { Name="Stamford", RegionalManagerAADGroupId="f4022ed6-4120-494f-813c-ef078d3876f8", SalespersonAADGroupId="16b56a6c-d017-40ee-b75b-a53ff7f9aa19" }
      };

      foreach(Branch b in branches) {
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

      foreach(Employee e in employees) {
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

      foreach(Salary s in salaries) {
        context.Salaries.Add(s);
      }
      context.SaveChanges();

      var sales = new Sale[] {
        new Sale { EmployeeID=3, Value=1000 },
        new Sale { EmployeeID=3, Value=2000 },
        new Sale { EmployeeID=4, Value=500 },
        new Sale { EmployeeID=4, Value=600 },
      };

      foreach(Sale s in sales) {
        context.Sales.Add(s);
      }
      context.SaveChanges();
    }
  }
}