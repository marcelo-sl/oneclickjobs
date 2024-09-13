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
public class ResumesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthenticationService _authenticationService;

    private const int MaximumResumes = 2;

    public ResumesController(ApplicationDbContext context, IAuthenticationService authenticationService)
    {
        _context = context;
        _authenticationService = authenticationService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _authenticationService.GetUserId();
        var resumes = await _context.Resumes
            .AsNoTracking()
            .Where(x => x.CreatedBy == userId)
            .ToListAsync();

        return View(resumes);
    }

    public async Task<IActionResult> Details(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await _context.Resumes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (resume == null)
        {
            return NotFound();
        }

        ViewResumeViewModel resumeViewModel = new()
        {
            Id = resume.Id,
            FileName = resume.FileName,
            Base64string = Convert.ToBase64String(resume.FileContent)
        };

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

        var userId = _authenticationService.GetUserId();

        var resumeCount = await _context.Resumes.AsNoTracking().Where(x => x.CreatedBy == userId).CountAsync();

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

        _context.Add(resume);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var resume = await _context.Resumes.FindAsync(id);
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
                _context.Update(resume);
                await _context.SaveChangesAsync();
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

        var resume = await _context.Resumes
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
        var resume = await _context.Resumes.FindAsync(id);
        if (resume != null)
        {
            _context.Resumes.Remove(resume);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ResumeExists(Guid id)
    {
        return _context.Resumes.Any(e => e.Id == id);
    }
}
