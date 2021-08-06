using System;
using System.Collections.Generic;

namespace AspNetCoreWithAppRoleAndFineGrained.Models {
  public class Branch {
    public int BranchID { get; set; }

    public string Name { get; set; }

    public string AADGroupID { get; set; }

    public virtual List<Employee> Employees { get; set; }
  }
}