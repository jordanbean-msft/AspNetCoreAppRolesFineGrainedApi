using System;
using System.Collections.Generic;

namespace AspNetCoreWithAppRolesAndFineGrained.Models {
  public class Employee {
    public int EmployeeID { get; set; }
    public int BranchID { get; set; }

    public string LastName { get; set; }

    public string FirstName { get; set; }

    public string UserPrincipalName { get; set; }

    public virtual ICollection<Sale> Sales { get; set; }

    public virtual Salary Salary { get; set; }

    public virtual Branch Branch { get; set; }
  }
}