using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Web.Models;
using PPI.Core.Web.Models.AmsaReports.Email;
using PPI.Core.Web.Models.AmsaReports.Email.ViewModel;
using PPI.Core.Web.Models.AmsaReports;
using System.IO;

namespace PPI.Core.Web.Controllers
{
    public class AMSAEmailsController : Controller
    {
        private AMSAReportContext dbr;		

		//Replace in Scafolding file
		//private AMSAReportContext db = new AMSAReportContext();

		public AMSAEmailsController()
        {
            dbr = new AMSAReportContext();
        }

        //Setup page (shows listing with buttons to edit)
        [HttpGet]
        public ActionResult Setup(int? id)
        {
            EmailListingViewModel evm = new EmailListingViewModel();
            int eventId = id ?? 0;
            if(eventId > 0)
            {
                evm.idSelectedEvent = eventId;
                evm.changeEvent();
            }
            return View(evm);
        }

        //Get page by id
        [HttpPost]
        public ActionResult Setup(EmailListingViewModel elvm)
        {
            if(elvm.idSelectedEvent > 0)
            {
                elvm.changeEvent();
            }
            return View(elvm);
        }


        [HttpPost]
        public ActionResult checkSanityForEvent(int eventId)
        {
            
            AMSAReportContext dbr = new AMSAReportContext();
            List<AMSAEmail> lstEmails = dbr.AMSAEmail.Where(m => m.AMSAEvent.id == eventId).ToList();
            bool setup = true;
            if(eventId > 0) { 
            int i = 0;
            //If e-maisl exist then check if they have something written in them
            if (lstEmails.Count > 0) { 
                while (setup &&  i < lstEmails.Count)
                {
                    //Check e-mails and let the user know if you find any problems
                    AMSAEmail currentEmail = lstEmails[i];
                        //For now check only invitation and reminder
                        if (currentEmail.Type.ToUpper().Equals("INVITATION") || currentEmail.Type.ToUpper().Equals("REMINDER"))
                        {
                            if (currentEmail.Closing == "" || currentEmail.Closing == null)
                                setup = false;
                            else if (currentEmail.DefaultFrom == "" || currentEmail.DefaultFrom == null)
                                setup = false;
                            else if (currentEmail.Introduction == "" || currentEmail.Introduction == null)
                                setup = false;
                            else if (currentEmail.Subject == "" || currentEmail.Subject == null)
                                setup = false;
                        }
                    i++;
                }
            }
            //If they don't exist then create them and let the user know that there is nothing written in them
            else
            {
                //Create emails
                AMSAEvent e = dbr.AMSAEvent.Find(eventId);
                //Generate e-mails leaving surveys@perfprog.com as the main e-mail address
                e.createEmails(e,e.defaultEmailAddress,dbr);
                setup = false;
            }
            dbr.Dispose();
            }
            else
            {
                setup = false;
            }

            if (setup)
            {
                return Json(new { error = false });
            }
            else
            {
                return Json(new { error = true });
            }
            
            
        }

        //Send (shows e-mails filtered by event) - No need to check which event has e-mails 
        //since they all do
        //Shows Invitation && Reminder
        [HttpGet]
        public ActionResult Send(int? eventId)
        {
            AMSAEmailSendViewModel esvm = new AMSAEmailSendViewModel();
            int id = eventId ?? 0;
            if(id > 0)
            {
                esvm.idSelectedEvent = id;
                esvm.loadParticipants();
            }
            return View(esvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetSendEvent(AMSAEmailSendViewModel svm)
        {
            svm.loadParticipants();
            return View("Send",svm);
        }

        //Edit e-mail
        [HttpGet]
        public ActionResult Edit(int id)
        {
            AMSAEmail e = dbr.AMSAEmail.Find(id);
            return View(e);
        }
        
        //Store edit for e-mail
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(AMSAEmail email)
        {
            email.saveChanges();
            EmailListingViewModel evm = new EmailListingViewModel();
            //Get the event and get the event id afterwards
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEmail e = dbr.AMSAEmail.Where(m => m.Id == email.Id).FirstOrDefault();
            evm.idSelectedEvent = e.AMSAEvent.id;
            evm.changeEvent();
            dbr.Dispose();
            return View("Setup", evm);
        }

        [HttpGet]
        //Preview e-mail that will be sent
        public ActionResult Preview(int id)
        {
            AMSAEmail e = dbr.AMSAEmail.Find(id);
            return View(e);
        }

        //Send E-mail

        /*
        SendEmail receives:
            lstId : id of users that will receive the e-mail
            eventId : id of the event the users are related to
            type : type of e-mail (Invitation or reminder) - depending on the type of e-mail and event what e-mail the constructor will get.
              can either be invite or reminder
            */
        [HttpPost]
        public ActionResult SendEmail(List<int> lstId, int eventId, string type)
        {
            AMSAEmail email = AMSAEmail.getEmail(eventId, type);
            List<string> lstEmailsErrors = new List<string>();
            List<AMSAParticipant> lstParticipants = new List<AMSAParticipant>();
            //Get list of participants we need to send e-mail to 
            foreach (int i in lstId)
            {
                lstParticipants.Add(dbr.AMSAParticipant.Where(m => m.Id == i && m.AMSAEvent.id == eventId).FirstOrDefault());
            }
            foreach(AMSAParticipant p in lstParticipants)
            {
                try
                {
                    email.send(p,this);
                    //Mark reminder and invitation date for participant
                    p.emailReceived(email.Type);
                }
                catch(Exception e)
                {
                    lstEmailsErrors.Add(p.PrimaryEmail);
                }   
            }
            //Return list of e-mails that we were not able to send to 
            if(lstEmailsErrors.Count == 0)
            {
                return Json(new
                {
                    error = false,
                    message = "All e-mails sent correctly!"
                });
            }

            //If there were no issues sending e-mails to the entire list then return errors = false
            if(lstEmailsErrors.Count > 0)
            {
                return Json(new
                {
                    error = true,
                    message = "Please make sure E-mail is setup with content and a subject. <br />Error sending e-mails to:",
                    lstEmails = lstEmailsErrors
                });
            }

            //Error 
            return Json(new
            {
                error = true,
                message = "System error please try again or inform IT department.",
                lstEmails = lstEmailsErrors
            });
        }

        /// <summary>
        /// Render a view into a string. It's a hack, it may fail badly.
        /// </summary>
        /// <param name="name">Name of the view, that is, its path.</param>
        /// <param name="data">Data to pass to the view, a model or something like that.</param>
        /// <returns>A string with the (HTML of) view.</returns>
        public string RenderPartialToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public ActionResult sendReminder()
        {
            //If reminder has not been sent today then send it
            if (!ReportUtilities.reminderSentToday())
            {
                try
                {
                    ReportUtilities.setReminderSentToday(); //Set today as the last date in which the reminder was sent
                    //Get all reminder e-mails
                    AMSAReportContext dbr = new AMSAReportContext();
                    List<AMSAEmail> lstReminderEmails = dbr.AMSAEmail.Where(m => m.Type.ToUpper().Equals("REMINDER")).ToList();
                    //On each e-mail execute method to send e-mails
                    foreach (AMSAEmail e in lstReminderEmails)
                    {
                        e.sendReminders(this);
                    }
                    dbr.Dispose();
                    return Json(new
                    {
                        error = false,
                        message = "emails sent"
                    });
                }
                catch
                {
                    return Json(new
                    {
                        error = true,
                        message = "error sending message"
                    });
                }
            }

            return Json(new
            {
                error = false,
                message = "reminder has already been sent today"
            });
        }


    }
}
