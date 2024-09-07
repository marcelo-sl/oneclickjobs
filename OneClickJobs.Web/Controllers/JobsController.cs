using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OneClickJobs.Web.Data.Contexts;
using OneClickJobs.Web.Data.Entities;
using OneClickJobs.Web.Data.Records;
using OneClickJobs.Web.Helpers;
using OneClickJobs.Web.Services.Authentication;

namespace OneClickJobs.Web.Controllers;

[Authorize]
public class JobsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthenticationService _authenticationService;

    public JobsController(ApplicationDbContext context, IAuthenticationService authenticationService)
    {
        _context = context;
        _authenticationService = authenticationService;
    }

    // GET: Jobs
    [AllowAnonymous]
    public async Task<IActionResult> Index([FromQuery] QueryParams queryParams)
    {
        List<Job> jobs = [];

        int recordsCount = 0;

        if (string.IsNullOrWhiteSpace(queryParams.Search))
        {
            recordsCount = _context.Jobs.Count();

            jobs = await _context.Jobs
                .AsNoTracking()
                .Skip(PaginationHelper.SkipRecords(queryParams.Page, queryParams.Size))
                .Take(PaginationHelper.TakeRecords(queryParams.Size))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }
        else
        {
            recordsCount = _context.Jobs
                .Where(x => x.Title.Contains(queryParams.Search) || x.Description.Contains(queryParams.Search))
                .Count();

            jobs = await _context.Jobs
                .AsNoTracking()
                .Where(x => x.Title.Contains(queryParams.Search) || x.Description.Contains(queryParams.Search))
                .Skip(PaginationHelper.SkipRecords(queryParams.Page, queryParams.Size))
                .Take(PaginationHelper.TakeRecords(queryParams.Size))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        ViewData["Search"] = queryParams.Search;
        ViewData["Count"] = recordsCount;

        return View(jobs);
    }

    // GET: Jobs/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await _context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return job == null ? NotFound() : View(job);
    }

    // GET: Jobs/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Jobs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Description")] Job job)
    {
        if (ModelState.IsValid)
        {
            job.CreatedBy = _authenticationService.GetUserId();
            _context.Add(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(job);
    }

    // GET: Jobs/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await _context.Jobs.FindAsync(id);
        return job == null ? NotFound() : View(job);
    }

    // POST: Jobs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Title,Description,Status,Id,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy")] Job job)
    {
        if (id != job.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(job);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobExists(job.Id))
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
        return View(job);
    }

    // GET: Jobs/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await _context.Jobs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (job == null)
        {
            return NotFound();
        }

        return View(job);
    }

    // POST: Jobs/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var job = await _context.Jobs.FindAsync(id);
        if (job != null)
        {
            _context.Jobs.Remove(job);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool JobExists(int id)
    {
        return _context.Jobs.AsNoTracking().Any(e => e.Id == id);
    }
}
