using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.GLMS.API.Data;
using TechMove.GLMS.API.Models;

namespace TechMove.GLMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContractsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agreement>>> GetContracts(
            [FromQuery] string? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var query = _context.Agreements.Include(a => a.Client).AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status.ToString() == status);
            }
            if (startDate.HasValue)
            {
                query = query.Where(a => a.StartDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(a => a.EndDate <= endDate.Value);
            }

            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Agreement>> GetContract(int id)
        {
            var contract = await _context.Agreements
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.AgreementId == id);

            if (contract == null)
            {
                return NotFound(new { message = $"Contract with id {id} not found" });
            }

            return contract;
        }

        [HttpPost]
        public async Task<ActionResult<Agreement>> PostContract(Agreement agreement)
        {
            var client = await _context.Clients.FindAsync(agreement.ClientId);
            if (client == null)
            {
                return BadRequest(new { error = "Client not found" });
            }

            _context.Agreements.Add(agreement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = agreement.AgreementId }, agreement);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] string status)
        {
            var contract = await _context.Agreements.FindAsync(id);
            if (contract == null)
            {
                return NotFound(new { message = $"Contract with id {id} not found" });
            }

            if (Enum.TryParse<AgreementStatus>(status, true, out var newStatus))
            {
                contract.Status = newStatus;
                await _context.SaveChangesAsync();
                return Ok(new { message = $"Contract status updated to {newStatus}", contract });
            }

            return BadRequest(new { error = "Invalid status. Use: Draft, Active, Expired, or OnHold" });
        }
    }
}