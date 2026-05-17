using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.GLMS.Web.Data;
using TechMove.GLMS.Web.Models;

namespace TechMove.GLMS.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("clients")]
        public async Task<ActionResult<int>> GetClientCount()
        {
            return await _context.Clients.CountAsync();
        }

        [HttpGet("agreements")]
        public async Task<ActionResult<int>> GetActiveAgreementCount()
        {
            return await _context.Agreements.CountAsync(a => a.Status == AgreementStatus.Active);
        }

        [HttpGet("requests")]
        public async Task<ActionResult<int>> GetServiceRequestCount()
        {
            return await _context.ServiceRequests.CountAsync();
        }
    }
}