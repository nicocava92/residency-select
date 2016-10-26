namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Events : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAEvents", "defaultEmailAddress", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AMSAEvents", "defaultEmailAddress", c => c.String());
        }
    }
}
