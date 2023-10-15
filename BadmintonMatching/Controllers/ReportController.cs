using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IReportServices _reportServices;

        public ReportController(IReportServices reportServices)
        {
            _reportServices = reportServices;
        }

        [HttpGet]
        [Route("{report_id}/detail")]
        public IActionResult GetReportDetail(int report_id)
        {
            //ReportDetail report = _reportServices.GetReportDetail(report_id);
            return Ok();
        }
    }
}
