using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports.Email
{
    /*
    E-mails that are created when an Event is Created
        TYPES OF E-MAILS:
         - INVITATION
         - REMINDER
         - REPORTS
         - ASSESSMENT
    */
    public class AMSAEmail
    {
        public int Id { get; set; }
        //Defines the type of e-mail
        public string Type { get; set; }
        public AMSAEvent AMSAEvent { get; set; }
        public string DefaultFrom { get; set; }
        public string Subject { get; set; }
        public string Introduction { get; set; }
        public string Closing { get; set; }

        public AMSAEmail(AMSAEvent e, string type, string defaultFrom, AMSAReportContext dbr)
        {
            this.AMSAEvent = e;
            this.Type = type;
            DefaultFrom = defaultFrom;
            Subject = "";
            Introduction = "";
            Closing = "";
            dbr.AMSAEmail.Add(this);
            dbr.SaveChanges();
        }

    }
}