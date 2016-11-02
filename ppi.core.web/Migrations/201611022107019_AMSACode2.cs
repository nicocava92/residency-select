namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSACode2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSACodes", new[] { "AMSAEvent_id" });
            AlterColumn("dbo.AMSACodes", "AMSAEvent_id", c => c.Int());
            CreateIndex("dbo.AMSACodes", "AMSAEvent_id");
            AddForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSACodes", new[] { "AMSAEvent_id" });
            AlterColumn("dbo.AMSACodes", "AMSAEvent_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSACodes", "AMSAEvent_id");
            AddForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents", "id", cascadeDelete: true);
        }
    }
}
