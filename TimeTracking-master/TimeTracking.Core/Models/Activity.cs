using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTracking.Core.Models;

namespace TimeTracking.Models
{
    public class Activity
    {
        public int ActivityID { get; set; }
        public string Name { get; set; }
        public virtual ActivityType ActivityType { get; set; }
        public TimeSpan WorkingTime { get; set; }
        public DateTime CreationDate { get; set; }
        public virtual Project Project { get; set; }
        
        public virtual ApplicationUser AssignedUser { get; set; }
        
        public virtual ApplicationUser Creator { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public ActivityStatus ActivityStatus { get; set; }
        public DateTime? FinalizationDate { get; set; }
    }


    public static class ActivitiesFilters
    {
        public static IQueryable<Activity> Finished(this IQueryable<Activity> activities)
        {
            return activities.Where(a => a.ActivityStatus == Core.Models.ActivityStatus.Finished);
        }

        public static IQueryable<Activity> OnInterval(this IQueryable<Activity> activities, DateTime finalDate, DateTime initialDate)
        {
            return activities.Where(a => a.FinalizationDate <= finalDate && a.FinalizationDate >= initialDate);
        }

        public static IQueryable<Activity> Doing(this IQueryable<Activity> activities)
        {
            return activities.Where(a => a.ActivityStatus == Core.Models.ActivityStatus.Created || a.ActivityStatus == Core.Models.ActivityStatus.Doing);
        }

    }

}
