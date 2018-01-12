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
            Activities = new List<Activity>();
        
        }
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string UserId { get; set; }
        public virtual Client Client { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
    }
}
