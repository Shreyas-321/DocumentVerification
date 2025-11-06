using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApitest.Data;
using webApitest.Models;

namespace webApitest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SampleDataController> _logger;

        public SampleDataController(ApplicationDbContext context, ILogger<SampleDataController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("insert-verification-sample-data")]
        public async Task<IActionResult> InsertVerificationSampleData()
        {
            try
            {
                // Clear existing sample data
                _context.OriginalAadhaarData.RemoveRange(_context.OriginalAadhaarData);
                _context.OriginalPANData.RemoveRange(_context.OriginalPANData);
                _context.OriginalECData.RemoveRange(_context.OriginalECData);
                _context.ExtractedData.RemoveRange(_context.ExtractedData);
                await _context.SaveChangesAsync();

                // Insert sample data here...
                await _context.SaveChangesAsync();

                return Ok(new { message = "Sample verification data inserted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inserting sample verification data");
                return StatusCode(500, "Internal server error while inserting sample data");
            }
        }
    }
}