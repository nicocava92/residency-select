namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAEvents", "StartDate", c => c.DateTime());
            AlterColumn("dbo.AMSAEvents", "ReviewDate", c => c.DateTime());
            AlterColumn("dbo.AMSAEvents", "CreateDate", c => c.DateTime());
            AlterColumn("dbo.AMSAEvents", "EndDate", c => c.DateTime());
            AlterColumn("dbo.AMSAEvents", "CompositeNeedByDate", c => c.DateTime());
            AlterColumn("dbo.AMSAEvents", "JetNeedByDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSAEvents", "JetNeedByDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "CompositeNeedByDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "EndDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "CreateDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "ReviewDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
