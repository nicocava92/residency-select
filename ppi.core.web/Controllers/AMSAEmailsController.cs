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
        public ActionResult Setup()
        {
            EmailListingViewModel elvm = new EmailListingViewModel();
            return View(elvm);
        }

        //Get page by id
        [HttpPost]
        public ActionResult Setup(EmailListingViewModel elvm)
        {
            if(elvm.idSelectedEvent > 0)
            {
                elvm.changeEvent();
                return View(elvm);
            }
            return View(elvm);
        }


        //Send (shows e-mails filtered by event) - No need to check which event has e-mails 
        //since they all do
        //Shows Invitation && Reminder
        [HttpGet]
        public ActionResult Send()
        {
            return View(new AMSAEmailSendViewModel());
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
            return View("Setup", dbr.AMSAEmail.OrderBy(m => m.AMSAEvent.id).ToList());
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
            try { 
                //Get all reminder e-mails
                AMSAReportContext dbr = new AMSAReportContext();
                List<AMSAEmail> lstReminderEmails = dbr.AMSAEmail.Where(m => m.Type.ToUpper().Equals("REMINDER")).ToList();
                    //On each e-mail execute method to send e-mails
                    foreach(AMSAEmail e in lstReminderEmails)
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
            catch { 
                return Json(new
                {
                    error = true,
                    message = "error sending message"
                });
            }
        }


    }
}
