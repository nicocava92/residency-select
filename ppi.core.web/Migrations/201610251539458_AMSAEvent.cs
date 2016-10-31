namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSAEvent : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes");
            DropIndex("dbo.AMSAEvents", new[] { "AMSAEventType_id" });
            AddColumn("dbo.AMSAEvents", "JetNeedByDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAEvents", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAEvents", "AMSAEventType_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSAEvents", "AMSAEventType_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes");
            DropIndex("dbo.AMSAEvents", new[] { "AMSAEventType_id" });
            AlterColumn("dbo.AMSAEvents", "AMSAEventType_id", c => c.Int());
            AlterColumn("dbo.AMSAEvents", "Description", c => c.String());
            AlterColumn("dbo.AMSAEvents", "Name", c => c.String());
            DropColumn("dbo.AMSAEvents", "JetNeedByDate");
            CreateIndex("dbo.AMSAEvents", "AMSAEventType_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes", "id");
        }
    }
}
