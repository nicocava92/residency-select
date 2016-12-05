using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using PPI.Core.Web.Controllers;
using System.Web.Mvc;

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
        

        internal void send(AMSAParticipant p, AMSAEmailsController controller)
        {
            /*******************************************************************************************
                Static text is set on the template for _PartialEmailInvitation or _PartialEmailReminder
            ********************************************************************************************/

            /*
                SEND E-MAIL USING PARTIAL E-MAIL FORMAT ALREADY CRAETED FOR HOGAN REPORTS
            */

            var EmailTemplate = new EmailTemplateModel();
            EmailTemplate.subject = this.Subject;
            EmailTemplate.closing = this.Closing;
            //Send over information about AMSA Code and AMSA Password
            EmailTemplate.amsa_id = p.AMSACode;
            EmailTemplate.amsa_password = p.AMSA_Password;
            EmailTemplate.introduction = this.Introduction;
            var Email = new EmailModel();
            Email.to = p.PrimaryEmail;
            Email.from = this.DefaultFrom;
            Email.subject = this.Subject;
            //Get data from the view reusing code form Emails controller created for Hogan reports
            
            if (this.Type.ToUpper().Equals("INVITATION"))
            {
                Email.body = controller.RenderPartialToString("_PartialEmailInvitation", EmailTemplate);
            }
            else if (this.Type.ToUpper().Equals("REMINDER"))
            {
                Email.body = controller.RenderPartialToString("_PartialEmailReminder", EmailTemplate);
            }

            //Send Grid example code
            var Credentials = new System.Net.NetworkCredential(
                    PPI.Core.Web.Properties.Settings.Default.SMTPUSER,
                    PPI.Core.Web.Properties.Settings.Default.SMTPPASSWORD
                    );

            var transportWeb = new SendGrid.Web(Credentials);

            var Mail = new SendGrid.SendGridMessage();

            Mail.AddTo(Email.to);
            Mail.From = new MailAddress(Email.from);


            Mail.Subject = Email.subject;
            Mail.Html = Email.body;


            transportWeb.Deliver(Mail);
        }

        internal void sendReminders(AMSAEmailsController controller)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Generate list of users that need to be sent the e-mail reminder for the event
            List<AMSAParticipant> lstParticipants = dbr.AMSAParticipant.Where(r => r.AMSAEvent.id == this.AMSAEvent.id).ToList();
            //Check if e-mail needs to be sent to the participant or not and send e-mail
            foreach (AMSAParticipant p in lstParticipants)
            {
                if (p.timeToSendReminder(this)) {
                    try { 
                        this.send(p, controller);
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