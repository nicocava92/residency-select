namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesForDashboard3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AmsaReportStudentDatas", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AmsaReportStudentDatas", "Status");
        }
    }
}
