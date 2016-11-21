namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesForDashboard2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSAEvents", "Updated", c => c.DateTime());
            AlterColumn("dbo.AmsaReportStudentDatas", "Updated", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AmsaReportStudentDatas", "Updated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AMSAEvents", "Updated", c => c.DateTime(nullable: false));
        }
    }
}
