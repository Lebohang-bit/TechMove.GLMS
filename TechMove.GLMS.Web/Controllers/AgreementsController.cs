using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.GLMS.Web.Data;
using TechMove.GLMS.Web.Models;

namespace TechMove.GLMS.Web.Controllers
{
    public class AgreementsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AgreementsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var agreements = _context.Agreements.Include(a => a.Client);
            return View(await agreements.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var agreement = await _context.Agreements.Include(a => a.Client).FirstOrDefaultAsync(m => m.AgreementId == id);
            if (agreement == null) return NotFound();
            return View(agreement);
        }

        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,StartDate,EndDate,Status,ServiceLevel,Terms")] Agreement agreement, IFormFile? pdfFile)
        {
            // Validate PDF file if provided
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var ext = Path.GetExtension(pdfFile.FileName).ToLower();
                if (ext != ".pdf")
                {
                    ModelState.AddModelError("pdfFile", $"ERROR: Only PDF files are allowed. You uploaded a {ext} file which is not accepted.");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }

                // Validate file size (optional - max 10MB)
                if (pdfFile.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("pdfFile", "File size exceeds 10MB limit. Please upload a smaller PDF.");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }

                try
                {
                    string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    string fileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
                    string filePath = Path.Combine(uploads, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await pdfFile.CopyToAsync(stream);
                    agreement.SignedAgreementPath = "/uploads/" + fileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("pdfFile", $"File upload failed: {ex.Message}");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }
            }

            // Validate date range
            if (agreement.EndDate < agreement.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be earlier than start date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(agreement);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Agreement created successfully for client ID {agreement.ClientId}";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
            return View(agreement);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var agreement = await _context.Agreements.FindAsync(id);
            if (agreement == null) return NotFound();
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
            return View(agreement);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgreementId,ClientId,StartDate,EndDate,Status,ServiceLevel,Terms,SignedAgreementPath")] Agreement agreement, IFormFile? pdfFile)
        {
            if (id != agreement.AgreementId) return NotFound();

            // Validate PDF file if provided
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var ext = Path.GetExtension(pdfFile.FileName).ToLower();
                if (ext != ".pdf")
                {
                    ModelState.AddModelError("pdfFile", $"ERROR: Only PDF files are allowed. You uploaded a {ext} file which is not accepted.");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }

                // Validate file size
                if (pdfFile.Length > 10 * 1024 * 1024)
                {
                    ModelState.AddModelError("pdfFile", "File size exceeds 10MB limit. Please upload a smaller PDF.");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }

                try
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(agreement.SignedAgreementPath))
                    {
                        string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, agreement.SignedAgreementPath.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                    }

                    string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    string fileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
                    string filePath = Path.Combine(uploads, fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await pdfFile.CopyToAsync(stream);
                    agreement.SignedAgreementPath = "/uploads/" + fileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("pdfFile", $"File upload failed: {ex.Message}");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }
            }

            // Validate date range
            if (agreement.EndDate < agreement.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be earlier than start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agreement);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Agreement updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgreementExists(agreement.AgreementId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
            return View(agreement);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var agreement = await _context.Agreements.Include(a => a.Client).FirstOrDefaultAsync(m => m.AgreementId == id);
            if (agreement == null) return NotFound();
            return View(agreement);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var agreement = await _context.Agreements.FindAsync(id);
            if (agreement != null)
            {
                // Delete associated PDF file
                if (!string.IsNullOrEmpty(agreement.SignedAgreementPath))
                {
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, agreement.SignedAgreementPath.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        TempData["Success"] = "Agreement and associated PDF file deleted successfully!";
                    }
                }
                _context.Agreements.Remove(agreement);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var agreement = await _context.Agreements.FindAsync(id);
            if (agreement == null || string.IsNullOrEmpty(agreement.SignedAgreementPath))
            {
                TempData["Error"] = "No PDF file found for this agreement.";
                return RedirectToAction(nameof(Index));
            }

            string path = Path.Combine(_webHostEnvironment.WebRootPath, agreement.SignedAgreementPath.TrimStart('/'));
            if (!System.IO.File.Exists(path))
            {
                TempData["Error"] = "PDF file not found on server.";
                return RedirectToAction(nameof(Index));
            }

            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/pdf", Path.GetFileName(agreement.SignedAgreementPath));
        }

        private bool AgreementExists(int id) => _context.Agreements.Any(e => e.AgreementId == id);
    }
}