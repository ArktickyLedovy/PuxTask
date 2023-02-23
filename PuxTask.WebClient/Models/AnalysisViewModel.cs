using PuxTask.Common.Entities;

namespace PuxTask.WebClient.Models
{
    public class AnalysisViewModel
    {
        public string analysedFolderPath { get; set; }
        public Report Report {get; set;}
        public string ErrorMessage { get; set; }
    }
}
