using PuxTask.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuxTask.Common.Entities
{
    public class FileReport
    {
        public string Name { get; set; }
        public int Version { get; set; }
        public FileState State { get; set; }
    }
}
