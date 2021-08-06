using System;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreWithAppRoleAndFineGrained.Models {
  public class Sale {
    public int SaleID { get; set; }
    public int EmployeeID { get; set; }
    public virtual Employee Employee { get; set;}
    [Display(Name = "Sale")]
    [DataType(DataType.Currency)]
    public int Value { get; set; }
  }
}