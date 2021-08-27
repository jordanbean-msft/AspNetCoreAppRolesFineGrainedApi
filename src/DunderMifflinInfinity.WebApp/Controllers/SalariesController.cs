using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DunderMifflinInfinity.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using DunderMifflinInfinity.WebApp.Services;
using Microsoft.Identity.Web;
using DunderMifflinInfinity.WebApp.AuthorizationHandlers;

namespace DunderMifflinInfinity.WebApp.Controllers
{
  [Authorize]
  [AuthorizeForScopes(ScopeKeySection = "DunderMifflinInfinity.Api:SalaryScope")]
  public class SalariesController : Controller
  {
    private readonly IAuthorizationService authorizationService;
    private readonly IEmployeeApiService employeeApiService;
    private readonly ISalaryApiService salaryApiService;

    public SalariesController(ISalaryApiService salaryApiService, IEmployeeApiService employeeApiService, IAuthorizationService authorizationService)
    {
      this.employeeApiService = employeeApiService;
      this.salaryApiService = salaryApiService;
      this.authorizationService = authorizationService;
    }

    // GET: Salaries
    [Authorize(Policy = Policies.GENERAL)]
    public async Task<IActionResult> Index()
    {
      return View(await salaryApiService.GetAsync());
    }

    // GET: Salaries/Details/5
    [Authorize(Policy = Policies.GENERAL)]
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      return View(await salaryApiService.GetAsync(id.Value));
    }

    // GET: Salaries/Create
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Create()
    {
      var employees = await employeeApiService.GetAsync();
      ViewData["EmployeeID"] = new SelectList(employees, "EmployeeID", "EmployeeID");
      return View();
    }

    // POST: Salaries/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Create([Bind("SalaryID,EmployeeID,Value")] Salary salary)
    {
      var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        if (ModelState.IsValid)
        {
          await salaryApiService.AddAsync(salary);
          return RedirectToAction(nameof(Index));
        }
        ViewData["EmployeeID"] = new SelectList(new List<Employee> { salary.Employee }, "EmployeeID", "EmployeeID", salary.EmployeeID);
        return View(salary);
      }
      else
      {
        return new ForbidResult();
      }
    }

    // GET: Salaries/Edit/5
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var salary = await salaryApiService.GetAsync(id.Value);
      if (salary == null)
      {
        return NotFound();
      }

      var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        ViewData["EmployeeID"] = new SelectList(new List<Employee>() { salary.Employee }, "EmployeeID", "EmployeeID", salary.EmployeeID);
        return View(salary);
      }
      else
      {
        return new ForbidResult();
      }
    }

    // POST: Salaries/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Edit(int id, [Bind("SalaryID,EmployeeID,Value")] Salary salary)
    {
      if (id != salary.SalaryID)
      {
        return NotFound();
      }

      //need to load the Employee & Branch navigation property      
      var existingSalary = await salaryApiService.GetAsync(id);

      var authorizationResult = await authorizationService.AuthorizeAsync(User, existingSalary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        if (ModelState.IsValid)
        {
          try
          {
            await salaryApiService.EditAsync(salary);
          }
          catch (Exception)
          {
            if (!SalaryExists(salary.SalaryID))
            {
              return NotFound();
            }
            else
            {
              throw;
            }
          }
          return RedirectToAction(nameof(Index));
        }
        ViewData["EmployeeID"] = new SelectList(new List<Employee>() { salary.Employee }, "EmployeeID", "EmployeeID", salary.EmployeeID);
        return View(salary);
      }
      else
      {
        return new ForbidResult();
      }
    }

    // GET: Salaries/Delete/5
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      var salary = await salaryApiService.GetAsync(id.Value);
      if (salary == null)
      {
        return NotFound();
      }

      var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        return View(salary);
      }
      else
      {
        return new ForbidResult();
      }
    }

    // POST: Salaries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var salary = await salaryApiService.GetAsync(id);

      var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        await salaryApiService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
      }
      else
      {
        return new ForbidResult();
      }
    }

    [Authorize(Policy = Policies.SALARY)]
    private bool SalaryExists(int id)
    {
      var salary = salaryApiService.GetAsync(id);

      if (salary != null)
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }
}
