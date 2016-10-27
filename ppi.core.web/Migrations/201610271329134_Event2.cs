namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSAEvents", "EventDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AMSAEvents", "EventDate");
        }
    }
}
