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
            List<AMSAEmail> lstEmails = dbr.AMSAEmail.OrderBy(m => m.AMSAEvent.id).ToList();
            return View(lstEmails);
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
        

    }
}
