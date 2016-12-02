namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSACodes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AMSACodes", "Pin", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AMSACodes", "Pin");
        }
    }
}
