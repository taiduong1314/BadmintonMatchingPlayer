﻿using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace BadmintonMatching.Controllers
{
    public enum ReportType
    {
        Post = 0,
        User = 1,
        Transaction = 2
    }

    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IReportServices _reportServices;
        private readonly IUserServices _userServices;

        public ReportController(IReportServices reportServices, IUserServices userServices)
        {
            _reportServices = reportServices;
            _userServices = userServices;
        }

        [HttpGet]
        [Route("status/{status}/{admin_id}")]
        [ProducesResponseType(typeof(SuccessObject<List<Reports>>), 200)]
        public async Task<IActionResult> GetReportByStatus(int admin_id, int status)
        {
            if (!_userServices.IsAdmin(admin_id))
            {
                return Ok(new SuccessObject<List<Reports?>> { Message = "Not admin" });
            }
            var reports = await _reportServices.GetByStatus((ReportStatus)status);
            return Ok(new SuccessObject<List<Reports>> { Data = reports, Message = Message.SuccessMsg });
        }

        [HttpPost]
        [Route("from_tran/{tran_id}")]
        public async Task<IActionResult> CreateFromTransaction(int tran_id, ReportContent info)
        {
            var report_id = await _reportServices.CreateFromTransaction(tran_id, info);
            if (report_id == 0)
            {
                return Ok(new SuccessObject<object> { Message = "Fail to create" });
            }
            else
            {
                return Ok(new SuccessObject<object> { Data = new { ReportId = report_id }, Message = Message.SuccessMsg });
            }
        }

        [HttpGet]
        [Route("type/{report_type}")]
        public async Task<IActionResult> GetReportByType(int report_type)
        {

        }

        //[HttpPost]
        //[Route("post/{post_id}")]
        //public async Task<IActionResult> ReportPost(int post_id)
        //{

        //}
    }
}
