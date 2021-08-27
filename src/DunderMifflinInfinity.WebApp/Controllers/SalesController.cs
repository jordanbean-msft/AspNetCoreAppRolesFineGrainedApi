using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DunderMifflinInfinity.WebApp.Models;
using DunderMifflinInfinity.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace DunderMifflinInfinity.WebApp.Controllers
{
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "DunderMifflinInfinity.Api:SaleScope")]
    public class SalesController : Controller
    {
        private readonly ISaleApiService saleApiService;
        private readonly IEmployeeApiService employeeApiService;
        public SalesController(ISaleApiService saleApiService, IEmployeeApiService employeeApiService)
        {
          this.employeeApiService = employeeApiService;
           this.saleApiService = saleApiService; 
        }

        // GET: Sales
        public async Task<IActionResult> Index()
        {            
            return View(await saleApiService.GetAsync());
        }

        // GET: Sales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await saleApiService.GetAsync(id.Value);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            var employees = await employeeApiService.GetAsync();
            ViewData["EmployeeID"] = new SelectList(employees, "EmployeeID", "EmployeeID");
            return View();
        }

        // POST: Sales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaleID,EmployeeID,Value")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                await saleApiService.AddAsync(sale);
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(new List<Employee>{sale.Employee}, "EmployeeID", "EmployeeID", sale.EmployeeID);
            return View(sale);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await saleApiService.GetAsync(id.Value);
            if (sale == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(new List<Employee>{sale.Employee}, "EmployeeID", "EmployeeID", sale.EmployeeID);
            return View(sale);
        }

        // POST: Sales/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaleID,EmployeeID,Value")] Sale sale)
        {
            if (id != sale.SaleID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await saleApiService.EditAsync(sale);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.SaleID))
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
            ViewData["EmployeeID"] = new SelectList(new List<Employee>{sale.Employee}, "EmployeeID", "EmployeeID", sale.EmployeeID);
            return View(sale);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await saleApiService.GetAsync(id.Value);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await saleApiService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            var sale = saleApiService.GetAsync(id);
            if(sale != null) {
              return true;
            }
            else
            {
              return false;
            }
        }
    }
}
