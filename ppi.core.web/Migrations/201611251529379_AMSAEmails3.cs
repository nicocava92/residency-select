namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSAEmails3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSAEmails", "automaticReminderDays", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AMSAEmails", "automaticReminderDays");
        }
    }
}
