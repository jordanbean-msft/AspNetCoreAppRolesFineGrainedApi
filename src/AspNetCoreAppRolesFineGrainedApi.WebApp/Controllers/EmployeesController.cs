using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Models;
using AspNetCoreAppRolesFineGrainedApi.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace AspNetCoreAppRolesFineGrainedApi.Controllers
{
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "AspNetCoreAppRolesFineGrainedApi.Api:DefaultScope")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeApiService employeeApiService;

        public EmployeesController(IEmployeeApiService employeeApiService)
        {
          this.employeeApiService = employeeApiService;   
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await employeeApiService.GetAsync());
        }

        // // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await employeeApiService.GetAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID,BranchID,LastName,FirstName")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                await employeeApiService.AddAsync(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // if (id == null)
            // {
            //     return NotFound();
            // }
 
            // var employee = await _context.Employees.FindAsync(id);
            // if (employee == null)
            // {
            //     return NotFound();
            // }
            // ViewData["BranchID"] = new SelectList(_context.Branches, "BranchID", "BranchID", employee.BranchID);
            return View();
        }

        // // POST: Employees/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to.
        // // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,BranchID,LastName,FirstName")] Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employee = await employeeApiService.EditAsync(employee);
                }
                catch (Exception)
                {
                    if (!await EmployeeExists(employee.EmployeeID))
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
            return View(employee);
        }

        // // GET: Employees/Delete/5
        // public async Task<IActionResult> Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var employee = await _context.Employees
        //         .Include(e => e.Branch)
        //         .FirstOrDefaultAsync(m => m.EmployeeID == id);
        //     if (employee == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(employee);
        // }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await employeeApiService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EmployeeExists(int id)
        {
            var employee = await employeeApiService.GetAsync(id);
            if(employee != null)
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
