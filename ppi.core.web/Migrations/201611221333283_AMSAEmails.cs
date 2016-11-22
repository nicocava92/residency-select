namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AMSAEmails : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AMSAEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                        DefaultFrom = c.String(),
                        Subject = c.String(),
                        Introduction = c.String(),
                        Closing = c.String(),
                        AMSAEvent_id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AMSAEvents", t => t.AMSAEvent_id)
                .Index(t => t.AMSAEvent_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSAEmails", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSAEmails", new[] { "AMSAEvent_id" });
            DropTable("dbo.AMSAEmails");
        }
    }
}
