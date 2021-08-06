using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AspNetCoreWithAppRoleAndFineGrained.Data;
using AspNetCoreWithAppRoleAndFineGrained.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using AspNetCoreWithAppRoleAndFineGrained.AuthorizationHandlers;
using AspNetCoreWithAppRoleAndFineGrained;
using AspNetCoreWithAppRolesAndFineGrained;

namespace web_app.Controllers
{
    [Authorize]
    public class SalariesController : Controller
    {
        private readonly AspNetCoreWithAppRoleAndFineGrainedDbContext _context;

        public SalariesController(AspNetCoreWithAppRoleAndFineGrainedDbContext context)
        {
            _context = context;
        }

        // GET: Salaries
        [Authorize(Policy = Policies.General)]
        public async Task<IActionResult> Index()
        {
            var salaries = new List<Salary>();

            if (User.IsInRole(AppRoles.CFO_READWRITE)) {
              salaries = await _context.Salaries.Include(salary => salary.Employee)
                                                .ToListAsync();
            } else if(User.IsInRole(AppRoles.REGIONAL_MANAGER_READWRITE)) {
              var aadGroupIds = User.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList<string>();

              salaries = await _context.Salaries.Include(salary => salary.Employee)
                                                .Where(salary => aadGroupIds.Contains(salary.Employee.Branch.RegionalManagerAADGroupId))
                                                .ToListAsync();
            } else if (User.IsInRole(AppRoles.SALESPERSON_READWRITE)) {
              string upn = User.Claims.FirstOrDefault(x => x.Type == "upn").Value;

              salaries = await _context.Salaries.Include(salary => salary.Employee)
                                                .Where(salary => salary.Employee.UserPrincipalName == upn)
                                                .ToListAsync();
            }            

            return View(salaries);
        }

        // GET: Salaries/Details/5
        [Authorize(Policy = Policies.General)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Salary salary = null;

            if (User.IsInRole(AppRoles.CFO_READWRITE)) {
              salary = await _context.Salaries.Include(salary => salary.Employee)
                                              .FirstOrDefaultAsync(m => m.SalaryID == id);
            } else if(User.IsInRole(AppRoles.REGIONAL_MANAGER_READWRITE)) {
              var aadGroupIds = User.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList<string>();

              salary = await _context.Salaries.Include(salary => salary.Employee)
                                                .Where(salary => aadGroupIds.Contains(salary.Employee.Branch.RegionalManagerAADGroupId))
                                                
                .FirstOrDefaultAsync(m => m.SalaryID == id);
            } else if (User.IsInRole(AppRoles.SALESPERSON_READWRITE)) {
              string upn = User.Claims.FirstOrDefault(x => x.Type == "upn").Value;

              salary = await _context.Salaries.Include(salary => salary.Employee)
                                                .Where(salary => salary.Employee.UserPrincipalName == upn)
                                                
                .FirstOrDefaultAsync(m => m.SalaryID == id);
            }  

            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // GET: Salaries/Create
        [Authorize(Policy = Policies.Management)]
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID");
            return View();
        }

        // POST: Salaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.Management)]
        public async Task<IActionResult> Create([Bind("SalaryID,EmployeeID,Value")] Salary salary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID", salary.EmployeeID);
            return View(salary);
        }

        // GET: Salaries/Edit/5
        [Authorize(Policy = Policies.Management)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID", salary.EmployeeID);
            return View(salary);
        }

        // POST: Salaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.Management)]
        public async Task<IActionResult> Edit(int id, [Bind("SalaryID,EmployeeID,Value")] Salary salary)
        {
            if (id != salary.SalaryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
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
            ViewData["EmployeeID"] = new SelectList(_context.Employees, "EmployeeID", "EmployeeID", salary.EmployeeID);
            return View(salary);
        }

        // GET: Salaries/Delete/5
        [Authorize(Policy = Policies.Management)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(m => m.SalaryID == id);
            if (salary == null)
            {
                return NotFound();
            }

            return View(salary);
        }

        // POST: Salaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.Management)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            _context.Salaries.Remove(salary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaryExists(int id)
        {
            return _context.Salaries.Any(e => e.SalaryID == id);
        }
    }
}
