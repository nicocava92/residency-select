using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
//Using related to AMSA Reports, getting data from different classes that participate with Events, Reports, Participants


namespace PPI.Core.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AmsaReports.AonItemsAF716AMSAVersionAB> lstReportItems { get; set; }
        public DbSet<AmsaReports.AmsaReportStudentData> lstStudentsForReport { get; set; }
        public DbSet<AmsaReports.AonParagraphs> lstAonParagraphs { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public System.Data.Entity.DbSet<PPI.Core.Web.Models.AmsaReports.AMSAEventStatus> AMSAEventStatus { get; set; }
    }




    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {        
        protected override void Seed(ApplicationDbContext context)
        {
            //var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            //var UserManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(context));
            //string name = "Admin";
            //string password = "test123";

            ////Create Role Admin if it does not exist
            //if (!RoleManager.RoleExists(name))
            //{
            //    var roleresult = RoleManager.Create(new IdentityRole(name));
            //}

            //var AdminUser = new ApplicationUser();
            //AdminUser.UserName = name;
            //var result = UserManager.Create(AdminUser,password);
            //if (result.Succeeded)
            //{
            //    var roleresult = UserManager.AddToRole(AdminUser.Id,name);
            //}


            //base.Seed(context);            
        }
    }

}