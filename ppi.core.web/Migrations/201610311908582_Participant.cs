namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Participant : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AMSAParticipants",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        PrimaryEmail = c.String(),
                        Gender = c.String(),
                        Title = c.String(),
                        AMSACode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AMSAParticipants");
        }
    }
}
