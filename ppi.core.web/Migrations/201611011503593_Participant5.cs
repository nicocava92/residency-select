namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant5 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AMSAParticipants", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSAParticipants", new[] { "AMSAEvent_id" });
            AlterColumn("dbo.AMSAParticipants", "AMSAEvent_id", c => c.Int());
            CreateIndex("dbo.AMSAParticipants", "AMSAEvent_id");
            AddForeignKey("dbo.AMSAParticipants", "AMSAEvent_id", "dbo.AMSAEvents", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAParticipants", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSAParticipants", new[] { "AMSAEvent_id" });
            AlterColumn("dbo.AMSAParticipants", "AMSAEvent_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSAParticipants", "AMSAEvent_id");
            AddForeignKey("dbo.AMSAParticipants", "AMSAEvent_id", "dbo.AMSAEvents", "id", cascadeDelete: true);
        }
    }
}
