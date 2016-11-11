namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSACode : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSACodes", new[] { "AMSAEvent_id" });
            AlterColumn("dbo.AMSACodes", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.AMSACodes", "AMSAEvent_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSACodes", "AMSAEvent_id");
            AddForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSACodes", new[] { "AMSAEvent_id" });
            AlterColumn("dbo.AMSACodes", "AMSAEvent_id", c => c.Int());
            AlterColumn("dbo.AMSACodes", "Code", c => c.String());
            CreateIndex("dbo.AMSACodes", "AMSAEvent_id");
            AddForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents", "id");
        }
    }
}
