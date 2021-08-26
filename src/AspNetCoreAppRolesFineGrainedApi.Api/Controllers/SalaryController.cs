using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreAppRolesFineGrainedApi.Api.Data;
using AspNetCoreAppRolesFineGrainedApi.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

namespace AspNetCoreAppRolesFineGrainedApi.Api.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [RequiredScope(RequiredScopesConfigurationKey = "ApiScopes:SalaryScope")]
  public class SalaryController : ControllerBase
  {
    private readonly IAuthorizationService _authorizationService;
    private readonly AspNetCoreAppRolesFineGrainedApiDbContext _context;

    public SalaryController(IAuthorizationService authorizationService, AspNetCoreAppRolesFineGrainedApiDbContext context)
    {
      _authorizationService = authorizationService;
      _context = context;
    }

    // GET: api/Salary
    [HttpGet]
    [Authorize(Policy = Policies.GENERAL)]
    public async Task<ActionResult<IEnumerable<Salary>>> GetSalaries()
    {
      var salaries = new List<Salary>();

      if (User.IsInRole(AppRoles.CFO_READWRITE))
      {
        salaries = await _context.Salaries.Include(salary => salary.Employee)
                                          .ToListAsync();
      }
      else if (User.IsInRole(AppRoles.REGIONAL_MANAGER_READWRITE))
      {
        var aadGroupIds = User.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList<string>();

        salaries = await _context.Salaries.Include(salary => salary.Employee)
                                          .Where(salary => aadGroupIds.Contains(salary.Employee.Branch.RegionalManagerAADGroupId))
                                          .ToListAsync();
      }
      else if (User.IsInRole(AppRoles.GENERAL_READWRITE))
      {
        string upn = User.Claims.FirstOrDefault(x => x.Type == "upn").Value;

        salaries = await _context.Salaries.Include(salary => salary.Employee)
                                          .Where(salary => salary.Employee.UserPrincipalName == upn)
                                          .ToListAsync();
      }
      return salaries;
    }

    // GET: api/Salary/5
    [HttpGet("{id}")]
    [Authorize(Policy = Policies.GENERAL)]
    public async Task<ActionResult<Salary>> GetSalary(int id)
    {
      Salary salary = null;

      if (User.IsInRole(AppRoles.CFO_READWRITE))
      {
        salary = await _context.Salaries.Include(salary => salary.Employee)
                                        .FirstOrDefaultAsync(m => m.SalaryID == id);
      }
      else if (User.IsInRole(AppRoles.REGIONAL_MANAGER_READWRITE))
      {
        var aadGroupIds = User.Claims.Where(x => x.Type == "groups").Select(x => x.Value).ToList<string>();

        salary = await _context.Salaries.Include(salary => salary.Employee)
                                          .Where(salary => aadGroupIds.Contains(salary.Employee.Branch.RegionalManagerAADGroupId))

          .FirstOrDefaultAsync(m => m.SalaryID == id);
      }
      else if (User.IsInRole(AppRoles.GENERAL_READWRITE))
      {
        string upn = User.Claims.FirstOrDefault(x => x.Type == "upn").Value;

        salary = await _context.Salaries.Include(salary => salary.Employee)
                                          .Where(salary => salary.Employee.UserPrincipalName == upn)

          .FirstOrDefaultAsync(m => m.SalaryID == id);
      }

      if (salary == null)
      {
        return NotFound();
      }

      return salary;
    }

    // PUT: api/Salary/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> PutSalary(int id, Salary salary)
    {
      if (id != salary.SalaryID)
      {
        return BadRequest();
      }

      //need to load the Employee & Branch navigation property      
      var existingSalary = await _context.Salaries.Where(s => s.SalaryID == salary.SalaryID)
                                                  .Include(s => s.Employee)
                                                  .Include(s => s.Employee.Branch)
                                                  .AsNoTracking()
                                                  .FirstAsync();

      var authorizationResult = await _authorizationService.AuthorizeAsync(User, existingSalary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {

        _context.Entry(salary).State = EntityState.Modified;

        try
        {
          await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!SalaryExists(id))
          {
            return NotFound();
          }
          else
          {
            throw;
          }
        }
      }
      else
      {
        return new ForbidResult();
      }

      return NoContent();
    }

    // POST: api/Salary
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<ActionResult<Salary>> PostSalary(Salary salary)
    {
      //need to load the Employee navigation property
      var employee = await _context.Employees.Where(employee => employee.EmployeeID == salary.EmployeeID)
                                             .Include(employee => employee.Branch)
                                             .AsNoTracking()
                                             .FirstAsync();
      salary.Employee = employee;

      var authorizationResult = await _authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        _context.Salaries.Add(salary);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSalary", new { id = salary.SalaryID }, salary);
      }
      else
      {
        return new ForbidResult();
      }
    }

    // DELETE: api/Salary/5
    [HttpDelete("{id}")]
    [Authorize(Policy = Policies.SALARY)]
    public async Task<IActionResult> DeleteSalary(int id)
    {
      var salary = await _context.Salaries.FindAsync(id);
      if (salary == null)
      {
        return NotFound();
      }
      //need to load the Employee navigation property
      _context.Salaries.Attach(salary);
      await _context.Entry(salary).Reference(s => s.Employee).LoadAsync();
      await _context.Entry(salary.Employee).Reference(e => e.Branch).LoadAsync();

      var authorizationResult = await _authorizationService.AuthorizeAsync(User, salary, Policies.SALARY);

      if (authorizationResult.Succeeded)
      {
        _context.Salaries.Remove(salary);
        await _context.SaveChangesAsync();
        return NoContent();
      }
      else
      {
        return new ForbidResult();
      }
    }

    [Authorize(Policy = Policies.SALARY)]
    private bool SalaryExists(int id)
    {
      return _context.Salaries.Any(e => e.SalaryID == id);
    }
  }
}
