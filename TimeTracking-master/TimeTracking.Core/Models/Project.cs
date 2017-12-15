using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace TimeTracking.Models
{
    public class Project
    {
        public Project() {
            ApplicationUsers = new List<ApplicationUser>();
            Activities = new List<Activity>();
        
        }
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
       
        public virtual ApplicationUser Owner { get; set; }

        public virtual ProjectType ProjectType { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }

        
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }

    }
}
