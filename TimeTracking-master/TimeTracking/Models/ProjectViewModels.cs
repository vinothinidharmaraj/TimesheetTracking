using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TimeTracking.Models
{
    public class IndexProjectViewModels
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        [Display(Name = "Creation Date")]
        public bool IsOwner { get; set; }
        public DateTime CreationDate { get; set; }
        public List<string> AppUsers { get; set; }
    }

    public class CreateProjectViewModel
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "Project Type")]
        public int TypeID { get; set; }

    }

    public class EditProjectViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public int TypeID { get; set; }

    }



    public class DetailsProjectViewModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        public string Owner { get; set; }
        public IEnumerable<string> Members { get; set; }
        public bool IsOwner { get; set; }
        public Dictionary<int, string> Activities { get; set; }

    }

    public class keyvaluePair
    {
        public int key { get; set; }
        public string value { get; set; }
    }
    public class DatesClass
    {
        public List<DateTime> weekDates { get; set; }
    }
    public class MemberViewModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    public class MembersProjectViewModel
    {
        [Required]
        public string IdMemberAdd { get; set; }
        public string Owner { get; set; }
        public IEnumerable<MemberViewModel> Members { get; set; }
        [Required]
        public int IdProject { get; set; }
    }

    public class RemoveMemberViewModel
    {
        public string IdMemberRemove { get; set; }
        public int IdProject { get; set; }

    }
}