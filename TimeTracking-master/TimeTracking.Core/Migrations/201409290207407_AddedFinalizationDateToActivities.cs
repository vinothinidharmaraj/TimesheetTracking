namespace TimeTracking.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFinalizationDateToActivities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Activities", "FinalizationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Activities", "FinalizationDate");
        }
    }
}
