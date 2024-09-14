using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using OneClickJobs.Domain.Entities;
using OneClickJobs.Domain.Services;
using OneClickJobs.Domain.ViewModels.Applications;
using OneClickJobs.Web.Data.Contexts;

namespace OneClickJobs.Web.Controllers;

public class ApplicationsController(ApplicationDbContext context, IAuthenticationService authenticationService) : Controller
{
    public async Task<IActionResult> Index()
    {
        return View(await context.Applications
            .AsNoTracking()
            .Include(x => x.Job)
            .Include(x => x.Resume)
            .Where(x => x.CreatedBy == authenticationService.GetUserId())
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync());
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var application = await context.Applications
            .AsNoTracking()
            .Include(x => x.Job)
            .Include (x => x.Resume)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (application == null)
        {
            return NotFound();
        }

        return View(application);
    }

    public async Task<IActionResult> Create(int? id)
    {
        var job = await context.Jobs.FindAsync(id);

        if (job == null)
            return NotFound();

        var userId = authenticationService.GetUserId();
        var resumes = await context.Resumes.AsNoTracking().Where(x => x.CreatedBy == userId).ToListAsync();

        ViewBag.UserAlreadyApplied = await context.Applications.AnyAsync(x => x.CreatedBy == userId && x.Job.Id == job.Id);
        ViewBag.Resumes = resumes.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FileName });
        ViewBag.Job = job;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateApplicationViewModel applicationViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(applicationViewModel);
        }

        var job = await context.Jobs.FindAsync(applicationViewModel.JobId);

        if (job == null)
        {
            ModelState.AddModelError("Job", "Job not exists.");
            return View(applicationViewModel);
        }

        var resume = await context.Resumes.FindAsync(applicationViewModel.ResumeId);

        if (resume == null)
        {
            ModelState.AddModelError("Resume", "Resume not exists.");
            return View(applicationViewModel);
        }

        var application = new Application()
        {
            Job = job,
            Resume = resume,
            CreatedBy = authenticationService.GetUserId(),
        };

        context.Applications.Add(application);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));            
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var application = await context.Applications
            .FirstOrDefaultAsync(m => m.Id == id);
        if (application == null)
        {
            return NotFound();
        }

        return View(application);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var application = await context.Applications.FindAsync(id);
        if (application != null)
        {
            context.Applications.Remove(application);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
