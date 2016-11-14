using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PPI.Core.Web.Models.AmsaReports;
using System.Data.Entity;
using PPI.Core.Web.Models.AmsaReports.Event;

namespace PPI.Core.Web.Models
{
    //Context class used for ABM of AMSA Information 
    // AMSA STUDENT DATA AND AON Tiem Report Data for generation is set in ApplicationDbContext
    public class AMSAReportContext:DbContext
    {
        public DbSet<AMSAEvent> AMSAEvent { get; set; }
        public DbSet<AMSAEventStatus> AMSAEventStatus { get; set; }
        public DbSet<AMSAEventType> AMSAEventType { get; set; }
        public DbSet<AMSAOrganization> AMSAOrganization { get; set; }
        public DbSet<AMSAProgram> AMSAProgram { get; set; }
        public DbSet<AMSAProgramSite> AMSAProgramSite { get; set; }
        public DbSet<AMSASite> AMSASite { get; set; }
        public DbSet<AMSASurveyType> AMSASurveySiteType { get; set; }
        public DbSet<AMSANotBillableReason> AMSANotBillableReason { get; set; }
        public DbSet<AMSAParticipant> AMSAParticipant { get; set; }
        //AMSA Codes - this list is also checked to see if the ammount of AMSA Codes in the application is too low
        public DbSet<AMSACode> AMSACodes { get; set; }

        //Datasets utilized for the genration of reports
        public DbSet<AmsaReports.AonItemsAF716AMSAVersionAB> lstReportItems { get; set; }
        public DbSet<AmsaReports.AmsaReportStudentData> lstStudentsForReport { get; set; }
        public DbSet<AmsaReports.AonParagraphs> lstAonParagraphs { get; set; }


        public AMSAReportContext()
            : base("DefaultConnection")
        {
        }
    }
}