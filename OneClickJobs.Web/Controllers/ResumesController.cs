using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using OneClickJobs.Domain.Entities;
using OneClickJobs.Domain.Services;
using OneClickJobs.Domain.ViewModels.Resumes;
using OneClickJobs.Web.Data.Contexts;
using OneClickJobs.Web.Helpers;

namespace OneClickJobs.Web.Controllers;

[Authorize]
public class ResumesController(ApplicationDbContext context, IAuthenticationService authenticationService) : Controller
{
    private const int MaximumResumes = 2;

    public async Task<IActionResult> Index()
    {
        var resumes = await context.Resumes
            .AsNoTracking()
            .Where(x => x.CreatedBy == authenticationService.GetUserId())
            .ToListAsync();

        return View(resumes);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await context.Resumes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (resume == null)
        {
            return NotFound();
        }

        var resumeViewModel = new ViewResumeViewModel(resume.Id, resume.FileName, resume.FileContent, resume.CreatedAt);

        return View(resumeViewModel);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateResumeViewModel createResumeViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(createResumeViewModel);
        }

        var userId = authenticationService.GetUserId();

        var resumeCount = await context.Resumes.AsNoTracking().Where(x => x.CreatedBy == userId).CountAsync();

        if (resumeCount >= MaximumResumes)
        {
            ModelState.AddModelError("Resume", "Only 2 resumes allowed.");
            return View(createResumeViewModel);
        }

        var resumeId = Guid.NewGuid();
        Resume resume = new()
        {
            Id = resumeId,
            CreatedBy = userId,
            FileName = createResumeViewModel.FormFile.FileName,
            FileContent = await FileHelper.ConvertToArrayAsync(createResumeViewModel.FormFile)
        };

        context.Add(resume);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await context.Resumes.FindAsync(id);
        if (resume == null)
        {
            return NotFound();
        }
        return View(resume);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, [Bind("FileName,FileContent,Id,CreatedAt,CreatedBy,UpdatedAt,UpdatedBy")] Resume resume)
    {
        if (id != resume.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(resume);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResumeExists(resume.Id))
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
        return View(resume);
    }

    public async Task<IActionResult> Delete(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await context.Resumes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (resume == null)
        {
            return NotFound();
        }

        return View(resume);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var resume = await context.Resumes.FindAsync(id);
        if (resume != null)
        {
            context.Resumes.Remove(resume);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ResumeExists(Guid id)
    {
        return context.Resumes.Any(e => e.Id == id);
    }
}
