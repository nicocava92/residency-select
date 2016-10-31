namespace PPI.Core.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovingImageInformationForAmsaSite : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AMSASites", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.AMSASites", "FriendlyName", c => c.String(nullable: false));
            DropColumn("dbo.AMSASites", "BrandingColor");
            DropColumn("dbo.AMSASites", "BrandingLogo");
            DropColumn("dbo.AMSASites", "BrandingLogoMimeType");
            DropColumn("dbo.AMSASites", "BrandingBackground");
            DropColumn("dbo.AMSASites", "BrandingBackgroundMimeType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AMSASites", "BrandingBackgroundMimeType", c => c.String());
            AddColumn("dbo.AMSASites", "BrandingBackground", c => c.String());
            AddColumn("dbo.AMSASites", "BrandingLogoMimeType", c => c.String());
            AddColumn("dbo.AMSASites", "BrandingLogo", c => c.String());
            AddColumn("dbo.AMSASites", "BrandingColor", c => c.String());
            AlterColumn("dbo.AMSASites", "FriendlyName", c => c.String());
            AlterColumn("dbo.AMSASites", "Name", c => c.String());
        }
    }
}
