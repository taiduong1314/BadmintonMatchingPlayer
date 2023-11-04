using Entities.Models;
using Entities.RequestObject;
using Entities.ResponseObject;
using Microsoft.EntityFrameworkCore;
using Repositories.Intefaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implements
{
    public class ReportServices : IReportServices
    {
        private readonly IRepositoryManager _repositoryManager;

        public ReportServices(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }
        public async Task<int> CreateFromTransaction(int tran_id, ReportContent info)
        {
            var tran = await _repositoryManager.Transaction.FindByCondition(x => x.Id == tran_id, false)
                .Include(x => x.IdSlotNavigation)
                .ThenInclude(x => x.IdPostNavigation)
                .FirstOrDefaultAsync();
            if (tran == null)
            {
                return 0;
            }
            else
            {
                var report = new Report
                {
                    IdTransaction = tran_id,
                    IdUserFrom = tran.IdUser,
                    IdUserTo = tran.IdSlotNavigation.IdPostNavigation.IdUserTo,
                    reportContent = info.reportContent,
                    ReportTitle  = info.ReportTitle,
                    Status = (int)ReportStatus.Pending,
                    TimeReport = DateTime.UtcNow,
                };
                _repositoryManager.Report.Create(report);
                await _repositoryManager.SaveAsync();
                return report.Id;
            }
        }

        public async Task<List<Reports>> GetByStatus(ReportStatus status)
        {
            switch (status)
            {
                case ReportStatus.Pending:
                case ReportStatus.Reviewing:
                    {
                        var reports = await _repositoryManager.Report.FindByCondition(x => x.Status == (int)status, false)
                            .Select(
                                x => new Reports
                                {
                                    Content = x.reportContent,
                                    Status = status.ToString(),
                                    DateReceive = x.TimeReport.Value.ToString("d"),
                                    Id = x.Id,
                                    Title = x.ReportTitle
                                })
                            .OrderBy(x => x.DateReceive)
                            .ToListAsync();
                        return reports;
                    }
                case ReportStatus.Resolved:
                    {
                        var reports = await _repositoryManager.Report.FindByCondition(x => x.Status == (int)status, false)
                            .Select(
                                x => new Reports
                                {
                                    Content = x.reportContent,
                                    Status = status.ToString(),
                                    DateReceive = x.TimeReport.Value.ToString("d"),
                                    Id = x.Id,
                                    Title = x.ReportTitle
                                })
                            .OrderByDescending(x => x.DateReceive)
                            .ToListAsync();
                        return reports;
                    }
                default:
                    {
                        var reports = await _repositoryManager.Report.FindAll(false)
                            .Select(
                                x => new Reports
                                {
                                    Content = x.reportContent,
                                    Status = ((ReportStatus)x.Status).ToString(),
                                    DateReceive = x.TimeReport.Value.ToString("d"),
                                    Id = x.Id,
                                    Title = x.ReportTitle
                                })
                            .OrderBy(x => x.DateReceive)
                            .ToListAsync();
                        return reports;
                    }
            }
        }
    }
}
