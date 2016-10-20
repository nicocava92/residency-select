namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StartingToAddEventsForAMSA : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AMSAEvents",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        ReviewDate = c.DateTime(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        Placement = c.Boolean(nullable: false),
                        Billable = c.Boolean(nullable: false),
                        AMSAEventStatus_id = c.Int(),
                        AMSAEventType_id = c.Int(),
                        AMSAProgramSite_id = c.Int(),
                        AMSASurveyType_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AMSAEventStatus", t => t.AMSAEventStatus_id)
                .ForeignKey("dbo.AMSAEventTypes", t => t.AMSAEventType_id)
                .ForeignKey("dbo.AMSAProgramSites", t => t.AMSAProgramSite_id)
                .ForeignKey("dbo.AMSASurveyTypes", t => t.AMSASurveyType_id)
                .Index(t => t.AMSAEventStatus_id)
                .Index(t => t.AMSAEventType_id)
                .Index(t => t.AMSAProgramSite_id)
                .Index(t => t.AMSASurveyType_id);
            
            CreateTable(
                "dbo.AMSAEventStatus",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AMSAEventTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AMSAProgramSites",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        AMSAProgram_id = c.Int(),
                        AMSASite_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AMSAPrograms", t => t.AMSAProgram_id)
                .ForeignKey("dbo.AMSASites", t => t.AMSASite_id)
                .Index(t => t.AMSAProgram_id)
                .Index(t => t.AMSASite_id);
            
            CreateTable(
                "dbo.AMSAPrograms",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AMSASites",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FriendlyName = c.String(),
                        BrandingColor = c.String(),
                        BrandingLogo = c.String(),
                        BrandingLogoMimeType = c.String(),
                        BrandingBackground = c.String(),
                        BrandingBackgroundMimeType = c.String(),
                        AMSAOrganization_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.AMSAOrganizations", t => t.AMSAOrganization_id)
                .Index(t => t.AMSAOrganization_id);
            
            CreateTable(
                "dbo.AMSAOrganizations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AMSASurveyTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSASurveyType_id", "dbo.AMSASurveyTypes");
            DropForeignKey("dbo.AMSAEvents", "AMSAProgramSite_id", "dbo.AMSAProgramSites");
            DropForeignKey("dbo.AMSAProgramSites", "AMSASite_id", "dbo.AMSASites");
            DropForeignKey("dbo.AMSASites", "AMSAOrganization_id", "dbo.AMSAOrganizations");
            DropForeignKey("dbo.AMSAProgramSites", "AMSAProgram_id", "dbo.AMSAPrograms");
            DropForeignKey("dbo.AMSAEvents", "AMSAEventType_id", "dbo.AMSAEventTypes");
            DropForeignKey("dbo.AMSAEvents", "AMSAEventStatus_id", "dbo.AMSAEventStatus");
            DropIndex("dbo.AMSASites", new[] { "AMSAOrganization_id" });
            DropIndex("dbo.AMSAProgramSites", new[] { "AMSASite_id" });
            DropIndex("dbo.AMSAProgramSites", new[] { "AMSAProgram_id" });
            DropIndex("dbo.AMSAEvents", new[] { "AMSASurveyType_id" });
            DropIndex("dbo.AMSAEvents", new[] { "AMSAProgramSite_id" });
            DropIndex("dbo.AMSAEvents", new[] { "AMSAEventType_id" });
            DropIndex("dbo.AMSAEvents", new[] { "AMSAEventStatus_id" });
            DropTable("dbo.AMSASurveyTypes");
            DropTable("dbo.AMSAOrganizations");
            DropTable("dbo.AMSASites");
            DropTable("dbo.AMSAPrograms");
            DropTable("dbo.AMSAProgramSites");
            DropTable("dbo.AMSAEventTypes");
            DropTable("dbo.AMSAEventStatus");
            DropTable("dbo.AMSAEvents");
        }
    }
}
