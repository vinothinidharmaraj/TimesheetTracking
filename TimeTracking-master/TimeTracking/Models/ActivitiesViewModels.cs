using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TimeTracking.Core.Models;

namespace TimeTracking.Models
{
    public class IndexActivityViewModel
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ActivityId { get; set; }
        [Display(Name = "Created by")]
        public string Creator { get; set; }
        [Display(Name = "Assigned User")]
        public string AssignedUser { get; set; }
        [Display(Name = "Status")]
        public string ActivityStatus { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }
        public TimeSpan WorkingTime { get; set; }
        public bool CanEdit { get; set; }
    }

    public class DetailsActivityViewModel
    {
        [Display(Name = "Id")]
        public int ActivityId { get; set; }
        public string Name { get; set; }
        [Display(Name = "Type")]
        public string ActivityType { get; set; }
        [Display(Name = "Status")]
        public string ActivityStatus { get; set; }
        [Display(Name = "Created by")]
        public string Creator { get; set; }
        [Display(Name = "Assigned by")]
        public string AssignedUser { get; set; }
        [Display(Name = "Working Time")]
        public TimeSpan WorkingTime { get; set; }
        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }
        public bool CanEdit { get; set; }
        public bool IsShowActivity { get; set; }
        public int ProjectId { get; set; }
        public DateTime TimeSheetDate { get; set; }
        public int NoOfHours { get; set; }
    }

    public class CreateActivityViewModel
    {
        [Display(Name = "Task")]
        public string Name { get; set; }
        [Display(Name = "Period")]
        public int ActivityType { get; set; }
        [Display(Name = "Reporting Manager")]
        public string RepManager { get; set; } = "Ramesh Maganti";
        //[Display(Name = "Assigned User")]
        //public string User { get; set; } = "Sameer";
        [Display(Name = "Working Time")]
        public TimeSpan WorkingTime { get; set; }
        public int ProjectId { get; set; }

    }

    public class TimesheetViewModel
    {
        public int ActivityId { get; set; }
        public int ProjectId { get; set; }
        public DateTime TimeSheetDate { get; set; }
        public int NoOfHours { get; set; }
    }

    public class EditActivityViewModel
    {
        [Display(Name = "Id")]
        public int ActivityId { get; set; }
        [Display(Name = "Type")]
        public int ActivityType { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Assigned User")]
        public string AssignedUserId { get; set; }
        public int ProjectId { get; set; }


    }

    public class CreateCommentsViewModel
    {
        public int ActivityId { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }


    }

    public class CommentsViewModel
    {
        public int CommentId { get; set; }
        [Display(Name = "Owner")]
        public string CommentOwner { get; set; }
        [Display(Name = "Comment")]
        [DisplayFormat()]
        public string Description { get; set; }
        [Display(Name = "Creation date")]
        public DateTime CreationDate { get; set; }

    }

    public class DeleteConfirmedViewModel
    {

        public string ActivityStatus { get; set; }
    }




}
