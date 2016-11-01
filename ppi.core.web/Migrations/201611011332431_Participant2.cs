namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSAParticipants", "AAMCNumber", c => c.String(nullable: false));
            AddColumn("dbo.AMSAParticipants", "AMSA_Password", c => c.String());
            AddColumn("dbo.AMSAParticipants", "AMSAEvent_id", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSAParticipants", "AMSAEvent_id");
            AddForeignKey("dbo.AMSAParticipants", "AMSAEvent_id", "dbo.AMSAEvents", "id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAParticipants", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSAParticipants", new[] { "AMSAEvent_id" });
            DropColumn("dbo.AMSAParticipants", "AMSAEvent_id");
            DropColumn("dbo.AMSAParticipants", "AMSA_Password");
            DropColumn("dbo.AMSAParticipants", "AAMCNumber");
        }
    }
}
