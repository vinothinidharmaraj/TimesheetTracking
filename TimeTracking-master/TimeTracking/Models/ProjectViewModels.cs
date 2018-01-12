using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TimeTracking.Models
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public string UserId { get; set; }
        public bool IsOwner { get; set; }
        public int ClientId { get; set; }
        public IEnumerable<string> Members { get; set; }
        public string PublicHolidays { get; set; }
        public List<DetailsActivityViewModel> Activities { get; set; }
    }

    public class IndexProjectViewModels
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Client { get; set; }
        [Display(Name = "Creation Date")]
        public bool IsOwner { get; set; }
        public DateTime CreationDate { get; set; }
        public List<string> AppUsers { get; set; }
    }

    public class CreateProjectViewModel
    {
        [Required]
        public string Name { get; set; }

        [Display(Name = "Client Name")]
        public int ClientId { get; set; }

    }

    public class EditProjectViewModel
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public int ClientId { get; set; }

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
        public string PublicHolidays { get; set; }
        public int ClientId { get; set; }
        public List<DetailsActivityViewModel> Activities { get; set; }
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