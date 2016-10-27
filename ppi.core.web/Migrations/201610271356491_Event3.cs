namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Event3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AMSAEvents", "EventDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AMSAEvents", "EventDate", c => c.DateTime(nullable: false));
        }
    }
}
