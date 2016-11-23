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
        public virtual AMSAEvent AMSAEvent { get; set; }
        [System.ComponentModel.DataAnnotations.Display(Name = "Default From")]
        public string DefaultFrom { get; set; }
        public string Subject { get; set; }
        public string Introduction { get; set; }
        public string Closing { get; set; }

        //parameterless constructor added for EF to load data from the database
        public AMSAEmail() { }

        //Used to get e-mail that will be sent
        public static AMSAEmail getEmail(int eventId, string type)
        {
            AMSAEmail ret = new AMSAEmail();
            AMSAReportContext dbr = new AMSAReportContext();
            if (type.ToUpper().Equals("INVITE"))
            {
                //Get invitation message
                ret = dbr.AMSAEmail.Where(m => m.AMSAEvent.id == eventId && m.Type.ToUpper().Equals("INVITATION")).FirstOrDefault();
            }
            if (type.ToUpper().Equals("Reminder"))
            {
                //Get reminder message
                ret = dbr.AMSAEmail.Where(m => m.AMSAEvent.id == eventId && m.Type.ToUpper().Equals("REMINDER")).FirstOrDefault();
            }
            dbr.Dispose();
            return ret;
        }

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

        internal void saveChanges()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEmail e = dbr.AMSAEmail.Find(this.Id);
            e.Type = e.Type;
            e.AMSAEvent = this.AMSAEvent;
            e.DefaultFrom = this.DefaultFrom;
            e.Subject = this.Subject;
            e.Introduction = this.Introduction;
            e.Closing = this.Closing;
            dbr.SaveChanges();
            dbr.Dispose();
        }

        internal void send(AMSAParticipant p)
        {
            
        }
    }
}