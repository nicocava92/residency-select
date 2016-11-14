namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Report : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AonParagraphs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        scale = c.String(),
                        scale_description = c.String(),
                        GATEField = c.String(),
                        paragraph = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.AonItemsAF716AMSAVersionAB",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Report = c.String(),
                        Label = c.String(),
                        GATEFields = c.String(),
                        VersionA = c.String(),
                        VersionB = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AmsaReportStudentDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        PersonId = c.String(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        CompletionDate = c.DateTime(nullable: false),
                        Stanine_Ambition = c.Int(nullable: false),
                        Stanine_Assertiveness = c.Int(nullable: false),
                        Stanine_Awareness = c.Int(nullable: false),
                        Stanine_Composure = c.Int(nullable: false),
                        Stanine_Conceptual = c.Int(nullable: false),
                        Stanine_Cooperativeness = c.Int(nullable: false),
                        Stanine_Drive = c.Int(nullable: false),
                        Stanine_Flexibility = c.Int(nullable: false),
                        Stanine_Humility = c.Int(nullable: false),
                        Stanine_Liveliness = c.Int(nullable: false),
                        Stanine_Mastery = c.Int(nullable: false),
                        Stanine_Positivity = c.Int(nullable: false),
                        Stanine_Power = c.Int(nullable: false),
                        Stanine_Sensitivity = c.Int(nullable: false),
                        Stanine_Structure = c.Int(nullable: false),
                        AMSAEvent_id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AMSAEvents", t => t.AMSAEvent_id)
                .Index(t => t.AMSAEvent_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AmsaReportStudentDatas", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AmsaReportStudentDatas", new[] { "AMSAEvent_id" });
            DropTable("dbo.AmsaReportStudentDatas");
            DropTable("dbo.AonItemsAF716AMSAVersionAB");
            DropTable("dbo.AonParagraphs");
        }
    }
}
