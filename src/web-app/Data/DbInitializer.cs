using AspNetCoreWithAppRoleAndFineGrained.Models;
using System;
using System.Linq;

namespace AspNetCoreWithAppRoleAndFineGrained.Data {
  public static class DbInitializer {
    public static void Initialize(AspNetCoreWithAppRoleAndFineGrainedDbContext context) {
      context.Database.EnsureCreated();

      if(context.Branches.Any()) {
        return; // DB has been seeded
      }

      var branches = new Branch[] {
        new Branch { Name="Scranton", AADGroupID="06dfb21a-ddbe-41de-bc8c-f334a2f0ed9b" },
        new Branch { Name="Stamford", AADGroupID="de22355d-bf15-4a8d-ac91-0529e395c365" }
      };

      foreach(Branch b in branches) {
        context.Branches.Add(b);
      }
      context.SaveChanges();

      var employees = new Employee[] {
        new Employee { FirstName="Michael",LastName="Scott",BranchID=1},
        new Employee { FirstName="Josh",LastName="Porter",BranchID=2},
        new Employee { FirstName="Dwight",LastName="Schrute",BranchID=1},
        new Employee { FirstName="Karen",LastName="Filippelli",BranchID=2}
      };

      foreach(Employee e in employees) {
        context.Employees.Add(e);
      }
      context.SaveChanges();

      var salaries = new Salary[] {
        new Salary { EmployeeID=1, Value=60000 },
        new Salary { EmployeeID=2, Value=80000 },
        new Salary { EmployeeID=3, Value=50000 },
        new Salary { EmployeeID=4, Value=50000 }
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