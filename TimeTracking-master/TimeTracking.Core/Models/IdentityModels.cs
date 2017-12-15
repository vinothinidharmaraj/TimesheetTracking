using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TimeTracking.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Project> Projects { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

            this.Database.Log = m => Debug.WriteLine(m);
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Project>().HasMany(p => p.ApplicationUsers).WithMany(au => au.Projects);

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<TimeTracking.Models.Project> Projects { get; set; }
        public System.Data.Entity.DbSet<TimeTracking.Models.ProjectType> ProjectTypes { get; set; }
        public System.Data.Entity.DbSet<TimeTracking.Models.Activity> Activities { get; set; }
        public System.Data.Entity.DbSet<TimeTracking.Models.ActivityType> ActivityTypes { get; set; }
        public System.Data.Entity.DbSet<TimeTracking.Core.Models.Comment> Comments { get; set; }
        public System.Data.Entity.DbSet<TimeTracking.Models.TimesheetData> Timedata { get; set; }
    }
}