using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Services;
using Microsoft.Identity.Web;

namespace AspNetCoreAppRolesFineGrainedApi.Controllers
{
  [Authorize]
  [AuthorizeForScopes(ScopeKeySection = "AspNetCoreAppRolesFineGrainedApi.Api:SalaryScope")]
  public class SalariesController : Controller
  {
    private readonly IAuthorizationService authorizationService;
    private readonly ISalaryApiService salaryApiService;

    public SalariesController(ISalaryApiService salaryApiService, IAuthorizationService authorizationService)
    {
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
    public IActionResult Create()
    {
      //TODO: fix this
      //ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID");
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
      // //need to load the Employee navigation property
      // var employee = await _context.Employees.Where(employee => employee.EmployeeID == salary.EmployeeID)
      //                                        .Include(employee => employee.Branch)
      //                                        .AsNoTracking()
      //                                        .FirstAsync();
      // salary.Employee = employee;
      
      // var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      // if (authorizationResult.Succeeded)
      // {
      //   if (ModelState.IsValid)
      //   {
      //     _context.Add(salary);
      //     await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
      //   }
      //   ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID", salary.EmployeeID);
      //   return View(salary);
      // }
      // else
      // {
      //   return new ForbidResult();
      // }
    }

    // GET: Salaries/Edit/5
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Edit(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // var salary = await _context.Salaries.FindAsync(id);
      // if (salary == null)
      // {
      //   return NotFound();
      // }
      
      // //need to load the Employee navigation property
      // _context.Salaries.Attach(salary);
      // await _context.Entry(salary).Reference(s => s.Employee).LoadAsync();
      // await _context.Entry(salary.Employee).Reference(e => e.Branch).LoadAsync();

      // var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      // if (authorizationResult.Succeeded)
      // {
      //   ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID", salary.EmployeeID);
      //   return View(salary);
      // }
      // else
      // {
        return new ForbidResult();
      //}
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
      // var existingSalary = await _context.Salaries.Where(s => s.SalaryID == salary.SalaryID)
      //                                             .Include(s => s.Employee)
      //                                             .Include(s => s.Employee.Branch)
      //                                             .AsNoTracking()
      //                                             .FirstAsync();

      // var authorizationResult = await authorizationService.AuthorizeAsync(User, existingSalary, Policies.SALARY);

      // if (authorizationResult.Succeeded)
      // {
      //   if (ModelState.IsValid)
      //   {
      //     try
      //     {
      //       _context.Update(salary);
      //       await _context.SaveChangesAsync();
      //     }
      //     catch (DbUpdateConcurrencyException)
      //     {
      //       if (!SalaryExists(salary.SalaryID))
      //       {
      //         return NotFound();
      //       }
      //       else
      //       {
      //         throw;
      //       }
      //     }
      //     return RedirectToAction(nameof(Index));
      //   }
      //   ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID", salary.EmployeeID);
        return View(salary);
      // }
      // else
      // {
      //   return new ForbidResult();
      // }
    }

    // GET: Salaries/Delete/5
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> Delete(int? id)
    {
      if (id == null)
      {
        return NotFound();
      }

      // var salary = await _context.Salaries
      //     .Include(s => s.Employee)
      //     .FirstOrDefaultAsync(m => m.SalaryID == id);
      // if (salary == null)
      // {
      //   return NotFound();
      // }
      
      // //need to load the Employee navigation property
      // _context.Salaries.Attach(salary);
      // await _context.Entry(salary).Reference(s => s.Employee).LoadAsync();
      // await _context.Entry(salary.Employee).Reference(e => e.Branch).LoadAsync();

      // var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      // if (authorizationResult.Succeeded)
      // {
        //return View(salary);
      // }
      // else
      // {
         return new ForbidResult();
      // }
    }

    // POST: Salaries/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      // var salary = await _context.Salaries.FindAsync(id);
      
      // //need to load the Employee navigation property
      // _context.Salaries.Attach(salary);
      // await _context.Entry(salary).Reference(s => s.Employee).LoadAsync();
      // await _context.Entry(salary.Employee).Reference(e => e.Branch).LoadAsync();

      // var authorizationResult = await authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      // if (authorizationResult.Succeeded)
      // {
      //   _context.Salaries.Remove(salary);
      //   await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      // }
      // else
      // {
      //   return new ForbidResult();
      // }
    }

    [Authorize(Policy = Policies.SALARY)]
    private bool SalaryExists(int id)
    {
      return true;//_context.Salaries.Any(e => e.SalaryID == id);
    }
  }
}
