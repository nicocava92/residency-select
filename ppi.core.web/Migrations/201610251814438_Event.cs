namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSAProgramSite_id", "dbo.AMSAProgramSites");
            DropIndex("dbo.AMSAEvents", new[] { "AMSAProgramSite_id" });
            AddColumn("dbo.AMSAEvents", "AMSAProgram_id", c => c.Int());
            AddColumn("dbo.AMSAEvents", "AMSASite_id", c => c.Int());
            AlterColumn("dbo.AMSAEvents", "Placement", c => c.Int(nullable: false));
            CreateIndex("dbo.AMSAEvents", "AMSAProgram_id");
            CreateIndex("dbo.AMSAEvents", "AMSASite_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAProgram_id", "dbo.AMSAPrograms", "id");
            AddForeignKey("dbo.AMSAEvents", "AMSASite_id", "dbo.AMSASites", "id");
            DropColumn("dbo.AMSAEvents", "AMSAProgramSite_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AMSAEvents", "AMSAProgramSite_id", c => c.Int());
            DropForeignKey("dbo.AMSAEvents", "AMSASite_id", "dbo.AMSASites");
            DropForeignKey("dbo.AMSAEvents", "AMSAProgram_id", "dbo.AMSAPrograms");
            DropIndex("dbo.AMSAEvents", new[] { "AMSASite_id" });
            DropIndex("dbo.AMSAEvents", new[] { "AMSAProgram_id" });
            AlterColumn("dbo.AMSAEvents", "Placement", c => c.Boolean(nullable: false));
            DropColumn("dbo.AMSAEvents", "AMSASite_id");
            DropColumn("dbo.AMSAEvents", "AMSAProgram_id");
            CreateIndex("dbo.AMSAEvents", "AMSAProgramSite_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAProgramSite_id", "dbo.AMSAProgramSites", "id");
        }
    }
}
