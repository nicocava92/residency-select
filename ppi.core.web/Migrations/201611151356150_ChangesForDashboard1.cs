namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesForDashboard1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmsaReportStudentDatas", "Uploaded", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AmsaReportStudentDatas", "Uploaded");
        }
    }
}
