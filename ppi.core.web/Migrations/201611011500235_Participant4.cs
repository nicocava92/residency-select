namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAParticipants", "Gender", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSAParticipants", "Gender", c => c.String(nullable: false));
        }
    }
}
