using System;
using System.Collections.Generic;

namespace AspNetCoreWithAppRoleAndFineGrained.Models {
  public class Branch {
    public int BranchID { get; set; }

    public string Name { get; set; }

    public string RegionalManagerAADGroupId { get; set; }
    public string SalespersonAADGroupId { get; set; }

    public virtual List<Employee> Employees { get; set; }
  }
}