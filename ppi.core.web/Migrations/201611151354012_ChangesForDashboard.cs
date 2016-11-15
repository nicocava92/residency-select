namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesForDashboard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSAEvents", "Updated", c => c.DateTime());
            AddColumn("dbo.AMSAParticipants", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AMSAParticipants", "Status");
            DropColumn("dbo.AMSAEvents", "Updated");
        }
    }
}
