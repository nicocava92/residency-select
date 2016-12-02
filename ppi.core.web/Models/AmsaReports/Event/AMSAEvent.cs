using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PPI.Core.Web.Models.AmsaReports.Event;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PPI.Core.Web.Models.AmsaReports.Email;
using System.Net.Mail;
using PPI.Core.Web.Controllers;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAEvent
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please enter a name for the event")]
        [DisplayName("Event Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a description for the event")]
        [DisplayName("Event Description")]
        public string Description { get; set; } 
        //Users should be able to upload and administer event types
        [DisplayName("Event Type")]
        public virtual AMSAEventType AMSAEventType { get; set; }  //Event type
        
        /*******
        Need to check how to add errors here
        *********/
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("Event / Review Date")]
        public DateTime? ReviewDate { get; set; }
        [DisplayName("Create Date")]
        public DateTime? CreateDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        //If Composite = true
        [DisplayName("Composite Needed By Date")]
        public DateTime? CompositeNeedByDate { get; set; }
        //If JetNeedByDate = true
        [DisplayName("Jet Needed By Date")]
        public DateTime? JetNeedByDate { get; set; }


        //Users should be able to upload and administer event status
        [DisplayName("Event Status")]
        public virtual AMSAEventStatus AMSAEventStatus { get; set; }

        public int Placement { get; set; }
        public bool Billable { get; set; } 

        [DisplayName("Organization")]
        public virtual AMSAOrganization AMSAOrganization { get; set; } //Organization

        [DisplayName("Survey Type")]
        public virtual AMSASurveyType AMSASurveyType { get; set; } // Survey Type

        [DisplayName("Speciality")]
        public virtual AMSAProgram AMSAProgram { get; set; } // Speciality

        [DisplayName("Department")]
        public virtual AMSASite AMSASite { get; set; } //Departement

        [DisplayName("Total Number of Participants")]
        [Required(ErrorMessage = "Please insert number of participants")]
        public int TotalNumberOfParticipants { get; set; } 

        [DisplayName("Default E-mail Address")]
        [EmailAddress(ErrorMessage = "Invalid E-mail Address")]
        [Required(ErrorMessage = "Please enter a Valid E-mail Address")]
        public string defaultEmailAddress { get; set; } 

        [DisplayName("Order By")]
        
        public string OrderBy { get; set; } 

        [DisplayName("Is Jet Required?")]
        public bool JetRequired { get; set; }
        [DisplayName("Composite Required?")]
        public bool CompositeRequired { get; set; }

        //If not billable select the reason
        [DisplayName("Not billable reason")]
        public virtual AMSANotBillableReason AMSANotBillableReason { get; set; }

        //If an event is not billable change the menu and expand information to add in why the event is not billable


        //Keeps track of when the event is upadted
        
        public DateTime? Updated { get; set; }

        public AMSAEvent()
        {
            Updated = DateTime.Now;
        }


        //Create e-mails related to the event
        public void createEmails(AMSAEvent e, string eMail, AMSAReportContext dbr)
        {
            List<string> emailTypes = new List<string>();
            emailTypes.Add("INVITATION");
            emailTypes.Add("REMINDER");
            emailTypes.Add("REPORT");
            emailTypes.Add("ASSESSMENT");
            foreach(string s in emailTypes)
            {
                AMSAEmail auxEmail = new AMSAEmail(e, s, eMail, dbr);
            }
            
            
        }
        //Once an Event is created an e-mail is sent to the defaul e-mail for the event informing that this event
        //Has been created, this is where that e-mail is sent
        internal void sendEventCreatedEmail(AMSAEventsController aMSAEventsController)
        {
            try
            {

                /*
                SEND E-MAIL USING PARTIAL E-MAIL FORMAT ALREADY CRAETED FOR HOGAN REPORTS
                */

                var EmailTemplate = new EmailTemplateModel();
                EmailTemplate.subject = "Your event " + this.Name + " has been created";
                EmailTemplate.closing = "You can now manage this event through the J3P Residency Select Administration portal.";
                EmailTemplate.introduction = "This email is to inform you that your event is now active. ";
                var Email = new EmailModel();
                Email.to = this.defaultEmailAddress;
                Email.from = "surveys@perfprog.com";
                Email.subject = EmailTemplate.subject;
                //Get data from the view reusing code form Emails controller created for Hogan reports
                Email.body = aMSAEventsController.RenderPartialToString("_PartialEmailFormat", EmailTemplate);

                //MailClass.SendEmail(emailmessage.Subject, emailmessage.Body, "noreply@j3personica.com", "nicocava92@live.com");


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
            catch(Exception e)
            {
                Console.WriteLine("Problem sending e-mail to default e-mail address provided");
            }
        }
        //Used to getAmmountOfCodes for the Event
        public int getAmmountCodes()
        {
            AMSAReportContext dbf = new AMSAReportContext();
            int ret = dbf.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == this.id).ToList().Count;
            dbf.Dispose();
            return ret;
        }
    }
}