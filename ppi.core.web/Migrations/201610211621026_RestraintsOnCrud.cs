namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestraintsOnCrud : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAEventStatus", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAEventTypes", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAPrograms", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAOrganizations", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AMSASurveyTypes", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSASurveyTypes", "Name", c => c.String());
            AlterColumn("dbo.AMSAOrganizations", "Name", c => c.String());
            AlterColumn("dbo.AMSAPrograms", "Name", c => c.String());
            AlterColumn("dbo.AMSAEventTypes", "Name", c => c.String());
            AlterColumn("dbo.AMSAEventStatus", "Name", c => c.String());
        }
    }
}
