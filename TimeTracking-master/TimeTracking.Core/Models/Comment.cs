using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeTracking.Models;

namespace TimeTracking.Core.Models
{
   public class Comment
    {
        public int CommentId { get; set; }
        public virtual ApplicationUser CommentOwner{ get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
