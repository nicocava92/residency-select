using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
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
        //Only for REMINDER Type e-mail
        //Amount of days to wait before sending automatic reminder
        [Display(Name = "Days before reminder")]
        public int automaticReminderDays { get; set; }

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
            if (type.ToUpper().Equals("REMINDER"))
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
            //Set automatic reminder to 7 days for REMINDER type e-mails, this can be changed from edit page
            if (this.Type.ToUpper().Equals("REMINDER"))
                automaticReminderDays = 7;
            else
                automaticReminderDays = 0;
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
            e.automaticReminderDays = this.automaticReminderDays;
            dbr.SaveChanges();
            dbr.Dispose();
        }

        //Sending e-mails to participants
        internal void send(AMSAParticipant p)
        {
            /***********************************
            MISSING INFORMATION THAT NEEDS TO BE SET FOR THE CONTENT OF THE E-MAIL
            ***********************************/
        

            var emailmessage = new System.Net.Mail.MailMessage();
            emailmessage.From = new System.Net.Mail.MailAddress(this.DefaultFrom);
            emailmessage.Subject = this.Subject;
            emailmessage.IsBodyHtml = true;
            emailmessage.Body = this.Introduction + this.Closing;
            //MailClass.SendEmail(emailmessage.Subject, emailmessage.Body, "noreply@j3personica.com", "nicocava92@live.com");


            //Send Grid example code
            var Credentials = new System.Net.NetworkCredential(
                    PPI.Core.Web.Properties.Settings.Default.SMTPUSER,
                    PPI.Core.Web.Properties.Settings.Default.SMTPPASSWORD
                    );

            var transportWeb = new SendGrid.Web(Credentials);

            var Mail = new SendGrid.SendGridMessage();

            MailAddress from = emailmessage.From;

            Mail.AddTo(p.PrimaryEmail);
            Mail.From = from;


            Mail.Subject = emailmessage.Subject;
            Mail.Html = emailmessage.Body;

            //We don't worry for errors at this point since if this raises an issue the top method has a try catch
            //That will return a message via json informing the encountered issues
            transportWeb.Deliver(Mail);
            
        }

        internal void sendReminders()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Generate list of users that need to be sent the e-mail reminder for the event
            List<AMSAParticipant> lstParticipants = dbr.AMSAParticipant.Where(r => r.AMSAEvent.id == this.AMSAEvent.id).ToList();
            //Check if e-mail needs to be sent to the participant or not and send e-mail
            foreach (AMSAParticipant p in lstParticipants)
            {
                if (p.timeToSendReminder(this)) {
                    try { 
                        this.send(p);
                    }
                    catch
                    {
                        Console.WriteLine("Let the user know that this e-mail was not sent correctly");
                    }
                }
            }

        }

    }
}