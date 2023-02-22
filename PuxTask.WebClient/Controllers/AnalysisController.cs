using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PuxTask.Abstract;
using PuxTask.WebClient.Models;

namespace PuxTask.WebClient.Controllers
{
    public class AnalysisController : Controller
    {
        private readonly IReportService _reportService;
        private readonly ILogger<AnalysisController> _logger;

        public AnalysisController(IReportService reportService, ILogger<AnalysisController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }
        public ActionResult Index()
        {
            return View(new AnalysisViewModel());
        }
        [HttpPost]
        public ActionResult Index(AnalysisViewModel vm) 
        {
            var startTime = DateTime.Now;
            _logger.LogInformation("Analysis started. Time: "+startTime.ToShortTimeString());
            vm.Report = _reportService.GetReports(vm.analysedFolderPath);
            _logger.LogInformation($"Analysis finished. " +
                $"Time: {DateTime.Now.ToShortTimeString()} " +
                $"Duration: {(DateTime.Now.Millisecond - startTime.Millisecond)} ms " +
                $"Files checked: {vm.Report.FileReports.Count}");
            return View(vm); 
        }
    }
}
