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
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var ext = Path.GetExtension(pdfFile.FileName).ToLower();
                if (ext != ".pdf")
                {
                    ModelState.AddModelError("pdfFile", "Only PDF files allowed");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }
                string uploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                string fileName = Guid.NewGuid().ToString() + "_" + pdfFile.FileName;
                string filePath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await pdfFile.CopyToAsync(stream);
                agreement.SignedAgreementPath = "/uploads/" + fileName;
            }
            if (ModelState.IsValid)
            {
                _context.Add(agreement);
                await _context.SaveChangesAsync();
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
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var ext = Path.GetExtension(pdfFile.FileName).ToLower();
                if (ext != ".pdf")
                {
                    ModelState.AddModelError("pdfFile", "Only PDF files allowed");
                    ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", agreement.ClientId);
                    return View(agreement);
                }
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
            if (ModelState.IsValid)
            {
                try { _context.Update(agreement); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!AgreementExists(agreement.AgreementId)) return NotFound(); else throw; }
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
                if (!string.IsNullOrEmpty(agreement.SignedAgreementPath))
                {
                    string path = Path.Combine(_webHostEnvironment.WebRootPath, agreement.SignedAgreementPath.TrimStart('/'));
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                _context.Agreements.Remove(agreement);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadFile(int id)
        {
            var agreement = await _context.Agreements.FindAsync(id);
            if (agreement == null || string.IsNullOrEmpty(agreement.SignedAgreementPath)) return NotFound();
            string path = Path.Combine(_webHostEnvironment.WebRootPath, agreement.SignedAgreementPath.TrimStart('/'));
            if (!System.IO.File.Exists(path)) return NotFound();
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/pdf", Path.GetFileName(agreement.SignedAgreementPath));
        }

        private bool AgreementExists(int id) => _context.Agreements.Any(e => e.AgreementId == id);
    }
}