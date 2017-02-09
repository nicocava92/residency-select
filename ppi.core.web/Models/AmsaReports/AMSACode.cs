using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSACode
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "AMSA Code is required")]
        [Display (Name = "GATE ID (User Id for AON)")]
        public string Code { get; set; }
        //Store the PIN (Password)t
        [Required (ErrorMessage = "PIN Required")]
        [Display(Name = "PIN (Password)")]
        public string Pin { get; set; }
        //Check if the AMSA Code is used
        public bool Used { get; set; }
        //AMSA Codes are related to an event
        public virtual AMSAEvent AMSAEvent { get; set; }

        public void markAsUsed()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            changeToTrue(dbr);
            int eventId = this.AMSAEvent.id;
            //Check if the ammount of codes is lower than 4, if it is then send perform user an e-mail
            List<AMSACode> lstCodes = dbr.AMSACodes.Where(m => m.AMSAEvent.id == eventId && !m.Used).ToList();
            if(lstCodes.Count < 4)
            {
                //Send e-mail letting the user know that the event is low on AMSA Codes

                var emailmessage = new System.Net.Mail.MailMessage();
                emailmessage.From = new System.Net.Mail.MailAddress("noreply@performanceprograms.com");
                emailmessage.Subject = "Running low on GATE ID (AMSA CODE) for " + this.AMSAEvent.Name;
                emailmessage.IsBodyHtml = true;
                emailmessage.Body = "<p>Less than 4 Gate (AMSA Codes) available for the event - "+this.AMSAEvent.Name+" - Ammount of AMAS codes left: "+lstCodes.Count+". <br />To add more codes onto the event please login to the system and go to AMSA Reports > Codes > Add Codes or Code Upload and select "+this.AMSAEvent.Name+" as the Event.</p>";
                //MailClass.SendEmail(emailmessage.Subject, emailmessage.Body, "noreply@j3personica.com", "nicocava92@live.com");

                //Send Grid example code
                var Credentials = new NetworkCredential(
                        PPI.Core.Web.Properties.Settings.Default.SMTPUSER,
                        PPI.Core.Web.Properties.Settings.Default.SMTPPASSWORD
                        );

                var transportWeb = new SendGrid.Web(Credentials);

                var Mail = new SendGrid.SendGridMessage();

                MailAddress from = new MailAddress("noreply@j3personica.com");

                ApplicationDbContext db = new ApplicationDbContext();
                string to = AMSAEvent.defaultEmailAddress;
                
                Mail.AddTo(to);
                Mail.From = from;


                Mail.Subject = emailmessage.Subject;
                Mail.Html = emailmessage.Body;
                try
                {
                    transportWeb.Deliver(Mail);
                }
                catch
                {
                    Console.WriteLine("Error producing message that will be sent");
                }

            }

            dbr.Dispose();
        }

        public void changeToTrue(AMSAReportContext dbr)
        {
            AMSACode c = dbr.AMSACodes.Find(this.Id);
            c.Used = true;
            dbr.SaveChanges();
            
        }
    }
}