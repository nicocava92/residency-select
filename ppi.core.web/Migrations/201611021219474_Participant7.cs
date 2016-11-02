namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAParticipants", "AMSACode", c => c.String(nullable: false));
            AlterColumn("dbo.AMSAParticipants", "AMSA_Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSAParticipants", "AMSA_Password", c => c.String());
            AlterColumn("dbo.AMSAParticipants", "AMSACode", c => c.String());
        }
    }
}
