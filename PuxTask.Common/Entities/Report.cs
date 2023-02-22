using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuxTask.Common.Entities
{
    public class Report
    {
        public Report(ICollection<FileReport> fileReports, string messageForUser)
        {
            FileReports = fileReports;
            MessageForUser = messageForUser;
        }

        public ICollection<FileReport> FileReports { get; set; }
        public string MessageForUser { get; set; }
    }
}
