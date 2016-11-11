namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAParticipants", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAParticipants", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAParticipants", "PrimaryEmail", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAParticipants", "Gender", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAParticipants", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSAParticipants", "Title", c => c.String());
            AlterColumn("dbo.AMSAParticipants", "Gender", c => c.String());
            AlterColumn("dbo.AMSAParticipants", "PrimaryEmail", c => c.String());
            AlterColumn("dbo.AMSAParticipants", "LastName", c => c.String());
            AlterColumn("dbo.AMSAParticipants", "FirstName", c => c.String());
        }
    }
}
