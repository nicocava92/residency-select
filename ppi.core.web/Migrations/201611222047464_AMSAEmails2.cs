namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSAEmails2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSAParticipants", "Invitation_date", c => c.DateTime());
            AddColumn("dbo.AMSAParticipants", "Reminder_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AMSAParticipants", "Reminder_date");
            DropColumn("dbo.AMSAParticipants", "Invitation_date");
        }
    }
}
