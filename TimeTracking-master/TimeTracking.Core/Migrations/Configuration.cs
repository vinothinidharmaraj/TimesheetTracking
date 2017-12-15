namespace TimeTracking.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using TimeTracking.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<TimeTracking.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "TimeTracking.Models.ApplicationDbContext";
        }

        protected override void Seed(TimeTracking.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            //context.ProjectTypes.AddOrUpdate(
            //    pt => pt.Description,
            //    new ProjectType {Description = "Type 1"},
            //    new ProjectType {Description = "Type 2"},
            //    new ProjectType {Description = "Type 3"},
            //    new ProjectType {Description = "Type 4"}
            //    );
        }
    }
}
