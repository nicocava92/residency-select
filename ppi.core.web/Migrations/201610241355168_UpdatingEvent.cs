namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSAEvents", "CompositeNeedByDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.AMSAEvents", "TotalNumberOfParticipants", c => c.Int(nullable: false));
            AddColumn("dbo.AMSAEvents", "defaultEmailAddress", c => c.String());
            AddColumn("dbo.AMSAEvents", "OrderBy", c => c.String());
            AddColumn("dbo.AMSAEvents", "JetRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.AMSAEvents", "CompositeRequired", c => c.Boolean(nullable: false));
            AddColumn("dbo.AMSAEvents", "AMSAOrganization_id", c => c.Int());
            CreateIndex("dbo.AMSAEvents", "AMSAOrganization_id");
            AddForeignKey("dbo.AMSAEvents", "AMSAOrganization_id", "dbo.AMSAOrganizations", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSAOrganization_id", "dbo.AMSAOrganizations");
            DropIndex("dbo.AMSAEvents", new[] { "AMSAOrganization_id" });
            DropColumn("dbo.AMSAEvents", "AMSAOrganization_id");
            DropColumn("dbo.AMSAEvents", "CompositeRequired");
            DropColumn("dbo.AMSAEvents", "JetRequired");
            DropColumn("dbo.AMSAEvents", "OrderBy");
            DropColumn("dbo.AMSAEvents", "defaultEmailAddress");
            DropColumn("dbo.AMSAEvents", "TotalNumberOfParticipants");
            DropColumn("dbo.AMSAEvents", "CompositeNeedByDate");
        }
    }
}
