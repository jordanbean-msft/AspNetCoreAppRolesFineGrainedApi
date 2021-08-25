using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreAppRolesFineGrainedApi.Models {
  public class Salary {
    public int SalaryID { get; set; }
    public int EmployeeID { get; set; }
    [Display(Name = "Salary")]
    [DataType(DataType.Currency)]
    public int Value { get; set; }

    public virtual Employee Employee { get; set; }
  }
}