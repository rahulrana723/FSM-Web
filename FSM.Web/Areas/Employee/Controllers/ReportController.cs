using FSM.Core.Interface;
using FSM.Core.ViewModels;
using FSM.Web.Areas.Employee.ViewModels;
using FSM.Web.Common;
using log4net;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSM.Web.Areas.Employee.Controllers
{
    public class ReportController : BaseController
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod
                                         ().DeclaringType);

        [Dependency]
        public ILogRepository logRepo { get; set; }

        [HttpGet]
        public ActionResult ActivityLog()
        {
            try
            {
                using (logRepo)
                {
                    LogViewModel model = new LogViewModel();

                    LogSearchViewModel logSearchViewModelPageSize = new LogSearchViewModel();
                    string Searchstring = Request.QueryString["searchkeyword"];
                    var logList = logRepo.GetLogDetail().ToList();

                    DateTime? StartDate = Request.QueryString["StartDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["StartDate"].ToString()) ?
                                  DateTime.Parse(Request.QueryString["StartDate"]) : (DateTime?)null : (DateTime?)null;
                    DateTime? EndDate = Request.QueryString["EndDate"] != null ? !string.IsNullOrEmpty(Request.QueryString["EndDate"].ToString()) ?
                               DateTime.Parse(Request.QueryString["EndDate"]) : (DateTime?)null : (DateTime?)null;


                    if (StartDate.HasValue && EndDate.HasValue)
                    {
                        logList = logList.Where(m => (m.Date != null && m.Date > StartDate && m.Date < EndDate)).ToList();
                        logSearchViewModelPageSize.StartDate = Convert.ToDateTime(StartDate.Value.ToString());
                        logSearchViewModelPageSize.EndDate = Convert.ToDateTime(EndDate.Value.ToString());
                    }

                    else if (StartDate.HasValue)
                    {
                        logList = logList.Where(m => (m.Date != null && m.Date >= StartDate)).ToList();
                        logSearchViewModelPageSize.StartDate = Convert.ToDateTime(StartDate.Value.ToString()); ;
                    }
                    else if (EndDate.HasValue)
                    {
                        logList = logList.Where(m => (m.Date != null && m.Date <= EndDate)).ToList();
                        logSearchViewModelPageSize.EndDate = Convert.ToDateTime(EndDate.Value.ToString()); ;
                    }

                    // mapping list<entity> to list<viewmodel>
                    CommonMapper<LogDetailsCoreViewModel, LogViewModel> mapper = new CommonMapper<LogDetailsCoreViewModel, LogViewModel>();
                    List<LogViewModel> logListing = mapper.MapToList(logList.ToList());
                    if (logListing.Count > 0)
                    {
                        logListing = mapper.MapToList(logList.ToList());

                    }
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                               Convert.ToInt32(Request.QueryString["page_size"]);

                    logSearchViewModelPageSize.PageSize = PageSize;
                    if (logSearchViewModelPageSize.searchkeyword == null)
                    {
                        logSearchViewModelPageSize.searchkeyword = string.IsNullOrEmpty(Searchstring) ? "" : Searchstring;
                    }

                    var logListViewModel = new LogListViewModel
                    {
                        logDetailList = logListing.OrderByDescending(i => i.Date),
                        logDetailInfo = logSearchViewModelPageSize
                    };
                    return View(logListViewModel);

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult ActivityLog(LogSearchViewModel logSearchViewModel)
        {
            try
            {
                using (logRepo)
                {
                    var logs = logRepo.GetLogDetail();


                    if (logSearchViewModel.StartDate != null && logSearchViewModel.EndDate != null)
                    {
                        logs = (DateTime)logSearchViewModel.StartDate != null
                                ? logs.Where(log => log.Date >= (DateTime)logSearchViewModel.StartDate
                                            && log.Date <= (DateTime)logSearchViewModel.EndDate)
                                : logs;
                    }

                    if (logSearchViewModel.StartDate == null && logSearchViewModel.EndDate != null)
                    {
                        logs = (DateTime)logSearchViewModel.EndDate != null ? logs.Where(log => log.Date
                                    <= (DateTime)logSearchViewModel.EndDate) : logs;
                    }
                    if (logSearchViewModel.StartDate != null && logSearchViewModel.EndDate == null)
                    {
                        logs = (DateTime)logSearchViewModel.StartDate != null ? logs.Where(log => log.Date
                                 >= (DateTime)logSearchViewModel.StartDate) : logs;
                    }

                    if (logSearchViewModel.searchkeyword != null)
                    {
                        logs = logSearchViewModel.searchkeyword != null ? logs.Where(
                            log => log.UserName.Contains(logSearchViewModel.searchkeyword) ||
                                   log.FullName.Contains(logSearchViewModel.searchkeyword) ||
                                   log.Message.Contains(logSearchViewModel.searchkeyword)
                            ) : logs;

                    }
                    CommonMapper<LogDetailsCoreViewModel, LogViewModel> mapper = new CommonMapper<LogDetailsCoreViewModel, LogViewModel>();
                    List<LogViewModel> logViewModel = mapper.MapToList(logs.ToList());
                    int PageSize = string.IsNullOrEmpty(Request.QueryString["page_size"]) ? 10 :
                                                Convert.ToInt32(Request.QueryString["page_size"]);
                    logSearchViewModel.PageSize = PageSize;
                    var logListViewModel = new LogListViewModel
                    {
                        logDetailList = logViewModel.OrderByDescending(i => i.Date),
                        logDetailInfo = logSearchViewModel
                    };

                    log4net.ThreadContext.Properties["UserId"] = base.GetUserId;
                    log.Info(base.GetUserName + " gets list of all user logs.");

                    return View(logListViewModel);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}