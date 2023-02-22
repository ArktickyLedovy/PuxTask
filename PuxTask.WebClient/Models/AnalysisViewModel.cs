using PuxTask.Common.Entities;

namespace PuxTask.WebClient.Models
{
    public class AnalysisViewModel
    {
        public string analysedFolderPath { get; set; }
        public ICollection<FileReport> Reports {get; set;}
    }
}
