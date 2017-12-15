using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TimeTracking.Models
{
    public class ProjectTotalHoursViewModel
    {
        public string Name { get; set; }
        public string Owner { get; set; }

        [Display(Name = "Total Hours")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double TotalHours { get; set; }
    }

    public class UserTotalHoursViewModel
    {

        public string Name { get; set; }
        [Display(Name = "Total Hours")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public double TotalHours { get; set; }
        public DateTime InitialDate { get; set; }
        [Display(Name = "Final Date")]
        [DataType(DataType.Date)]
        public DateTime FinalDate { get; set; }

    }

    public class SetPeriodViewModel
    {
        [Display(Name = "Initial Date")]
        [DataType(DataType.Date)]
        public DateTime InitialDate { get; set; }
        [Display(Name = "Final Date")]
        [DataType(DataType.Date)]
        public DateTime FinalDate { get; set; }
        public List<ProjectTotalHoursViewModel> ResultProjects { get; set; }
        public List<UserTotalHoursViewModel> ResultUsers { get; set; }
        public string Owner { get; set; }
        public string Name { get; set; }

    }


}