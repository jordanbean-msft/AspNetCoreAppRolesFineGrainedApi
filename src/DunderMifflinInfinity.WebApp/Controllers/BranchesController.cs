using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DunderMifflinInfinity.WebApp.Models;
using DunderMifflinInfinity.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace DunderMifflinInfinity.WebApp.Controllers
{
    [Authorize]
    [AuthorizeForScopes(ScopeKeySection = "DunderMifflinInfinity.Api:DefaultScope")]
    public class BranchesController : Controller
    {
        private readonly IBranchApiService branchApiService;

        public BranchesController(IBranchApiService branchApiService)
        {
          this.branchApiService = branchApiService;   
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            return View(await branchApiService.GetAsync());
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await branchApiService.GetAsync(id.Value);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Branches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BranchID,Name,AADGroupID")] Branch branch)
        {
            if (ModelState.IsValid)
            {
                await branchApiService.AddAsync(branch);
                return RedirectToAction(nameof(Index));
            }
            return View(branch);
        }

        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await branchApiService.GetAsync(id.Value);
            if (branch == null)
            {
                return NotFound();
            }
            return View(branch);
        }

        // POST: Branches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BranchID,Name,AADGroupID")] Branch branch)
        {
            if (id != branch.BranchID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await branchApiService.EditAsync(branch);
                }
                catch (Exception)
                {
                    if (!await BranchExists(branch.BranchID))
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
            return View(branch);
        }

        // GET: Branches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await branchApiService.GetAsync(id.Value);
            if (branch == null)
            {
                return NotFound();
            }

            return View(branch);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await branchApiService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            var branch = await branchApiService.GetAsync(id);
            if(branch != null)
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
