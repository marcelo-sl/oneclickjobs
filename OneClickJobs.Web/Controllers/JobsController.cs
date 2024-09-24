using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using OneClickJobs.Domain.Entities;
using OneClickJobs.Domain.Services;
using OneClickJobs.Domain.ViewModels.Jobs;
using OneClickJobs.Web.Data.Contexts;

namespace OneClickJobs.Web.Controllers;

[Authorize]
public class JobsController(ApplicationDbContext context, IAuthenticationService authenticationService, IMemoryCache memoryCache) : Controller
{
    private const string jobsKey = "jobs";

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        if (!memoryCache.TryGetValue(jobsKey, out var cachedJobs))
        {
            List<Job> jobs = jobs = await context.Jobs
                .AsNoTracking()
                .Include(x => x.Categories)
                .Where(x => x.Status == JobStatus.Open)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            memoryCache.Set(jobsKey, jobs, TimeSpan.FromMinutes(5));
            return View(jobs);
        }

        return View(cachedJobs);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await context.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        return job == null ? NotFound() : View(job);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateJobViewModel jobViewModel)
    {
        if (ModelState.IsValid)
        {
            var userId = authenticationService.GetUserId();

            Job newJob = new()
            {
                Title = jobViewModel.Title,
                Description = jobViewModel.Description,
                CreatedBy = userId
            };

            var category = await context.Categories
                .Where(x => x.Name == jobViewModel.Category)
                .FirstOrDefaultAsync();

            if (category == null)
            {
                Category newCategory = new()
                {
                    Name = jobViewModel.Category,
                    CreatedBy = userId
                };

                context.Categories.Add(newCategory);
                category = newCategory;
            }

            newJob.Categories.Add(category);

            context.Add(newJob);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(jobViewModel);
    }

    // GET: Jobs/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await context.Jobs.FindAsync(id);
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
                context.Update(job);
                await context.SaveChangesAsync();
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

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var job = await context.Jobs
            .FirstOrDefaultAsync(m => m.Id == id);
        if (job == null)
        {
            return NotFound();
        }

        return View(job);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var job = await context.Jobs.FindAsync(id);
        if (job != null)
        {
            context.Jobs.Remove(job);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool JobExists(int id)
    {
        return context.Jobs.AsNoTracking().Any(e => e.Id == id);
    }
}
