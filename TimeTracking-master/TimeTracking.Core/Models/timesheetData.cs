using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracking.Models
{
    public class TimesheetData
    {
        public int ProjectId { get; set; }
        public DateTime TimeSheetDate { get; set; }
        public int NoOfHours { get; set; }
        public int ActivityId { get; set; }
        [Key]
        public int timesheetID { get; set; }
    }
}
