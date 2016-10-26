namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes");
            DropIndex("dbo.AMSAEvents", new[] { "AMSAEventType_id" });
            AlterColumn("dbo.AMSAEvents", "AMSAEventType_id", c => c.Int());
            CreateIndex("dbo.AMSAEvents", "AMSAEventType_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes");
            DropIndex("dbo.AMSAEvents", new[] { "AMSAEventType_id" });
            AlterColumn("dbo.AMSAEvents", "AMSAEventType_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSAEvents", "AMSAEventType_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes", "id", cascadeDelete: true);
        }
    }
}
