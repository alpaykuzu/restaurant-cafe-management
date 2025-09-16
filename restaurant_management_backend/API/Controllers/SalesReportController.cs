using Core.Dtos.SalesReportDtos;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class SalesReportController : ControllerBase
    {
        private readonly ISalesReportService _salesReportService;

        public SalesReportController(ISalesReportService salesReportService)
        {
            _salesReportService = salesReportService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateReport(CreateSalesReportRequestDto request)
        {
            return Ok(await _salesReportService.GenerateReportAsync(request));
        }
    }
}
