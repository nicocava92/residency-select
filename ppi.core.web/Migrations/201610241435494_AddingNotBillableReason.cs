namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingNotBillableReason : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AMSANotBillableReasons",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("dbo.AMSAEvents", "AMSANotBillableReason_id", c => c.Int());
            CreateIndex("dbo.AMSAEvents", "AMSANotBillableReason_id");
            AddForeignKey("dbo.AMSAEvents", "AMSANotBillableReason_id", "dbo.AMSANotBillableReasons", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAEvents", "AMSANotBillableReason_id", "dbo.AMSANotBillableReasons");
            DropIndex("dbo.AMSAEvents", new[] { "AMSANotBillableReason_id" });
            DropColumn("dbo.AMSAEvents", "AMSANotBillableReason_id");
            DropTable("dbo.AMSANotBillableReasons");
        }
    }
}
