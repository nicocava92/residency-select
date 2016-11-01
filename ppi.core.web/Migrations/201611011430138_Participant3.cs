namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAParticipants", "AMSA_Password", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSAParticipants", "AMSA_Password", c => c.String());
        }
    }
}
