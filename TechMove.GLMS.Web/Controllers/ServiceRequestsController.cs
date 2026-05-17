using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TechMove.GLMS.Web.Data;
using TechMove.GLMS.Web.Models;

namespace TechMove.GLMS.Web.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index(string? status, DateTime? start, DateTime? end)
        {
            var requests = _context.ServiceRequests.Include(s => s.Agreement).ThenInclude(a => a.Client).AsQueryable();
            if (start.HasValue) requests = requests.Where(s => s.RequestDate >= start.Value);
            if (end.HasValue) requests = requests.Where(s => s.RequestDate <= end.Value);
            if (!string.IsNullOrEmpty(status) && status != "All" && Enum.TryParse<RequestStatusType>(status, out var stat))
                requests = requests.Where(s => s.Status == stat);
            ViewBag.CurrentStatus = status;
            ViewBag.CurrentStart = start?.ToString("yyyy-MM-dd");
            ViewBag.CurrentEnd = end?.ToString("yyyy-MM-dd");
            return View(await requests.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.ServiceRequests.Include(s => s.Agreement).ThenInclude(a => a.Client).FirstOrDefaultAsync(m => m.ServiceRequestId == id);
            if (request == null) return NotFound();
            return View(request);
        }

        public async Task<IActionResult> Create()
        {
            var activeAgreements = await _context.Agreements.Include(a => a.Client).Where(a => a.Status == AgreementStatus.Active).ToListAsync();
            ViewBag.AgreementId = new SelectList(activeAgreements, "AgreementId", "Client.Name");
            if (!activeAgreements.Any()) TempData["Error"] = "No active agreements found.";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgreementId,Description,AmountUSD,RequestDate,Status")] ServiceRequest serviceRequest)
        {
            var agreement = await _context.Agreements.FindAsync(serviceRequest.AgreementId);
            if (agreement == null || agreement.Status != AgreementStatus.Active)
            {
                ModelState.AddModelError("", "Cannot create request. Agreement must be ACTIVE.");
                await PopulateAgreementList();
                return View(serviceRequest);
            }
            if (ModelState.IsValid)
            {
                decimal rate = await GetExchangeRateAsync();
                serviceRequest.ExchangeRateUsed = rate;
                serviceRequest.AmountZAR = serviceRequest.AmountUSD * rate;
                if (serviceRequest.Status == 0) serviceRequest.Status = RequestStatusType.Pending;
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Request created! Rate: 1 USD = {rate} ZAR";
                return RedirectToAction(nameof(Index));
            }
            await PopulateAgreementList();
            return View(serviceRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.ServiceRequests.Include(s => s.Agreement).FirstOrDefaultAsync(s => s.ServiceRequestId == id);
            if (request == null) return NotFound();
            var activeAgreements = await _context.Agreements.Where(a => a.Status == AgreementStatus.Active).ToListAsync();
            ViewBag.AgreementId = new SelectList(activeAgreements, "AgreementId", "Client.Name", request.AgreementId);
            return View(request);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceRequestId,AgreementId,Description,AmountUSD,AmountZAR,RequestDate,Status,ExchangeRateUsed")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.ServiceRequestId) return NotFound();
            var agreement = await _context.Agreements.FindAsync(serviceRequest.AgreementId);
            if (agreement?.Status != AgreementStatus.Active)
            {
                ModelState.AddModelError("", "Cannot edit. Agreement must be ACTIVE.");
                await PopulateAgreementList();
                return View(serviceRequest);
            }
            if (ModelState.IsValid)
            {
                try { _context.Update(serviceRequest); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!RequestExists(serviceRequest.ServiceRequestId)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            await PopulateAgreementList();
            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var request = await _context.ServiceRequests.Include(s => s.Agreement).ThenInclude(a => a.Client).FirstOrDefaultAsync(m => m.ServiceRequestId == id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request != null) _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await GetExchangeRateAsync();
            return Json(new { rate });
        }

        private async Task<decimal> GetExchangeRateAsync()
        {
            try
            {
                string url = "https://api.exchangerate-api.com/v4/latest/USD";
                var response = await _httpClient.GetStringAsync(url);
                var data = JObject.Parse(response);
                var rate = data["rates"]?["ZAR"]?.Value<decimal>();
                return rate ?? 19.50m;
            }
            catch { return 19.50m; }
        }

        private async Task PopulateAgreementList()
        {
            var active = await _context.Agreements.Include(a => a.Client).Where(a => a.Status == AgreementStatus.Active).ToListAsync();
            ViewBag.AgreementId = new SelectList(active, "AgreementId", "Client.Name");
        }

        private bool RequestExists(int id) => _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
    }
}