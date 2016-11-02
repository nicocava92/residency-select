namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AMSACodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Used = c.Boolean(nullable: false),
                        AMSAEvent_id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AMSAEvents", t => t.AMSAEvent_id)
                .Index(t => t.AMSAEvent_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AMSACodes", "AMSAEvent_id", "dbo.AMSAEvents");
            DropIndex("dbo.AMSACodes", new[] { "AMSAEvent_id" });
            DropTable("dbo.AMSACodes");
        }
    }
}
