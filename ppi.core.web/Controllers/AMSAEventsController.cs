using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;

using PPI.Core.Web.Models;
using PPI.Core.Web.Models.AmsaReports;
using PPI.Core.Web.Models.AmsaReports.Event.ViewModel;
using System.Web.Mvc;
using System.IO;
using System.Net.Mail;

namespace PPI.Core.Web.Controllers
{
    public class AMSAEventsController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSAEventsController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAEvents/
        public ActionResult Index()
        {
            var model = dbr.AMSAEvent.ToList();
			return View(model);
        }

        // GET: /AMSAEvents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAEvent aMSAEvent = db.AMSAEvent.Find(id);
			var model = dbr.AMSAEvent.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAEvents/Create
        public ActionResult Create()
        {
            AMSAEventViewModel ae = new AMSAEventViewModel();
            ae.AMSAEvent.OrderBy = User.Identity.Name;
            if (Session["ae"] != null)
                ae = (AMSAEventViewModel)Session["ae"];
            return View(ae);
        }

        // POST: /AMSAEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AMSAEventViewModel ae)
        {
            if (Session["ae"] != null)
                ae = (AMSAEventViewModel)Session["ae"];
            //Check for select menus that are missing a selection
            else
                ae.selectMenusHaveValues(ModelState);


            if (ModelState.IsValid) //Need to change this because all information will not be present because of pagination
            {
                //db.AMSAEvent.Add(aMSAEvent);
                //db.SaveChanges();

                /************
                Instaed of creating right away we should send them to a page where they can view what the entered, give them the opportunity
                to come back or finish entering the Event
                ***************/
                Session["ae"] = ae;
                return View("CreateInsertDates", ae);
            }

            return View(ae);
        }

        //Get page that shows date insertion for the different values related to Amsa Event


        [HttpGet]
        public ActionResult CreateInsertDates(AMSAEventViewModel ae) {
            ae = (AMSAEventViewModel)Session["ae"];
            return View(ae);
        }
        //After dates are inserted show preview craete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInsertDatesPost(AMSAEventViewModel ae) {
            AMSAEventViewModel aeForInsert = (AMSAEventViewModel)Session["ae"];
            //Assing the dates to the model that is stored in the session variable
            aeForInsert.AMSAEvent.CompositeNeedByDate = ae.AMSAEvent.CompositeNeedByDate;
            aeForInsert.AMSAEvent.CreateDate = ae.AMSAEvent.CreateDate;
            aeForInsert.AMSAEvent.EndDate = ae.AMSAEvent.EndDate;
            aeForInsert.AMSAEvent.JetNeedByDate = ae.AMSAEvent.JetNeedByDate;
            aeForInsert.AMSAEvent.ReviewDate = ae.AMSAEvent.ReviewDate;
            aeForInsert.AMSAEvent.StartDate = ae.AMSAEvent.StartDate;
            Session["ae"] = aeForInsert;
            ae.AMSAEvent.Name = aeForInsert.AMSAEvent.Name;
            ae.AMSAEvent.defaultEmailAddress = aeForInsert.AMSAEvent.defaultEmailAddress;
            ae.AMSAEvent.Description = aeForInsert.AMSAEvent.Description;
            //If only 3 errors are present they are related to errors from other views
            ae.checkDates(ModelState);
            if (ModelState.IsValid) { 
                return RedirectToAction("PreviewCreate");
            }
            return View("CreateInsertDates",aeForInsert);
        }

        //Get page that shows a preview of the information that will be stored for the AMSA Event at hand (if accepted stores)
        [HttpGet]
        public ActionResult PreviewCreate()
        {
            AMSAEventViewModel ae = (AMSAEventViewModel)Session["ae"];
            return View(ae);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Stores the event in the database
        public ActionResult CompleteEventInsertion()
        {
            //Gets value from the datbase for the event
            AMSAEventViewModel ae = (AMSAEventViewModel)Session["ae"];
            ae.AMSAEvent.CreateDate = DateTime.Now;
            
            //Check all of it for errors, if no errors are found then store the Event in the db
            Session["ae"] = ae;
            ae.saveNewEvent();
            sendEventCreatedEmails(ae.AMSAEvent);
            //ae.AMSAEvent.sendEventCreatedEmail(this);
            Session["ae"] = null;
            return View("Complete");
        }

        private void sendEventCreatedEmails(AMSAEvent e)
        {
            try
            {

                /*
                SEND E-MAIL USING PARTIAL E-MAIL FORMAT ALREADY CRAETED FOR HOGAN REPORTS
                */

                var EmailTemplate = new EmailTemplateModel();
                System.Text.StringBuilder subject = new System.Text.StringBuilder();
                subject.Append("Your event ");
                subject.Append(e.Name);
                subject.Append(" has been created");
                EmailTemplate.subject = subject.ToString();
                EmailTemplate.closing = "You can now manage this event through the J3P Residency Select Administration portal.";
                EmailTemplate.introduction = "This email is to inform you that your event is now active. ";
                var Email = new EmailModel();
                Email.to = e.defaultEmailAddress;
                Email.from = "surveys@perfprog.com";
                Email.subject = EmailTemplate.subject;
                //Get data from the view reusing code form Emails controller created for Hogan reports
                Email.body = RenderPartialToString("_PartialEmailForAdministrators", EmailTemplate);

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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

        [HttpGet]
        public ActionResult Complete()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult GoBackToInsertDates(AMSAEventViewModel ae)
        {
            return RedirectToAction("CreateInsertDates", ae);
        }

        [HttpGet]
        public ActionResult GoBackToInitialCreate(AMSAEventViewModel ae)
        {
            return RedirectToAction("Craete", ae);
        }

        // GET: /AMSAEvents/Edit/5
        public ActionResult Edit(int id)
        {   
            //AMSAEvent aMSAEvent = db.AMSAEvent.Find(id);
			AMSAEvent model = dbr.AMSAEvent.First(m => m.id == id);
            AMSAEventViewModel ae = new AMSAEventViewModel();
            ae.loadSelectedData(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(ae);
        }

        // POST: /AMSAEvents/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AMSAEventViewModel ae)
        {
            ae.checkDates(ModelState);
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAEvent).State = EntityState.Modified;
                //db.SaveChanges();

                //Storing changes performed to the ASMA Event
                ae.saveChanges();
				return RedirectToAction("Index");
            }
            return View(ae);
        }

        // GET: /AMSAEvents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAEvent aMSAEvent = db.AMSAEvent.Find(id);
			var model = dbr.AMSAEvent.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSAEvent aMSAEvent = db.AMSAEvent.Find(id);
           // db.AMSAEvent.Remove(aMSAEvent);
           // db.SaveChanges();
		    var model = dbr.AMSAEvent.First(m => m.id == id);
		    dbr.AMSAEvent.Remove(model);
            dbr.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}
           // base.Dispose(disposing);
        }

        //Method called to get values for when Organization changes
        [HttpPost]
        public ActionResult GetDepartments(int organizationId) {
            //Search for departments (sites) with the selected organization Id and change the values inside of the select menu
            List<AMSASite> lstResult = new List<AMSASite>();
            lstResult = dbr.AMSASite.Where(m => m.AMSAOrganization.id == organizationId).ToList();
            return Json( new { Data = lstResult });
        }

        [HttpPost]
        public ActionResult GetSpeciality(int departmentId)
        {
            List<AMSAProgram> lstResult = new List<AMSAProgram>();
            //Store the ids of programs related to the location
            List<int> lstIdLocation = new List<int>();
            List<AMSAProgramSite> lstProgramSite = dbr.AMSAProgramSite.Where(m => m.AMSASite.id == departmentId).ToList();
            foreach(var a in lstProgramSite)
            {
                lstIdLocation.Add(a.AMSAProgram.id);
            }
            //Get all programs and filter them
            foreach(int i in lstIdLocation)
            {
                AMSAProgram auxProgram = dbr.AMSAProgram.Where(m => m.id == i).FirstOrDefault();
                if (!lstResult.Contains(auxProgram))
                {
                    lstResult.Add(auxProgram);
                }
            }
            return Json(new { Data = lstResult });
        }
    }

}