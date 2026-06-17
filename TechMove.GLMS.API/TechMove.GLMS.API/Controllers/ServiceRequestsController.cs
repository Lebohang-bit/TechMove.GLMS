using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using TechMove.GLMS.API.Data;
using TechMove.GLMS.API.Models;

namespace TechMove.GLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;

        public ServiceRequestsController(ApplicationDbContext context)
        {
            _context = context;
            _httpClient = new HttpClient();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRequest>>> GetServiceRequests(
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _context.ServiceRequests
                .Include(s => s.Agreement)
                .ThenInclude(a => a.Client)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status.ToString() == status);
            }
            if (startDate.HasValue)
            {
                query = query.Where(s => s.RequestDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(s => s.RequestDate <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetServiceRequest(int id)
        {
            var request = await _context.ServiceRequests
                .Include(s => s.Agreement)
                .ThenInclude(a => a.Client)
                .FirstOrDefaultAsync(s => s.ServiceRequestId == id);

            if (request == null)
            {
                return NotFound(new { message = $"Service request with id {id} not found" });
            }

            return request;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> PostServiceRequest(ServiceRequest serviceRequest)
        {
            var agreement = await _context.Agreements.FindAsync(serviceRequest.AgreementId);
            if (agreement == null)
            {
                return BadRequest(new { error = "Agreement not found" });
            }

            if (agreement.Status != AgreementStatus.Active)
            {
                return BadRequest(new { error = $"Cannot create request. Agreement is {agreement.Status}. Only ACTIVE agreements can have service requests." });
            }

            var rate = await GetExchangeRateAsync();
            serviceRequest.ExchangeRateUsed = rate;
            serviceRequest.AmountZAR = serviceRequest.AmountUSD * rate;

            if (serviceRequest.Status == 0)
            {
                serviceRequest.Status = RequestStatusType.Pending;
            }

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceRequest), new { id = serviceRequest.ServiceRequestId }, serviceRequest);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRequestStatus(int id, [FromBody] string status)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound(new { message = $"Service request with id {id} not found" });
            }

            if (Enum.TryParse<RequestStatusType>(status, true, out var newStatus))
            {
                request.Status = newStatus;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Request status updated to {newStatus}", request });
            }

            return BadRequest(new { error = "Invalid status" });
        }

        [HttpGet("exchangerate")]
        [AllowAnonymous]
        public async Task<IActionResult> GetExchangeRate()
        {
            var rate = await GetExchangeRateAsync();
            return Ok(new { rate });
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
            catch
            {
                return 19.50m;
            }
        }
    }
}