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
            Session["ae"] = null;
            return View("Complete");
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
            //Store the ids of programs related to the department
            List<int> lstIdDepartment = new List<int>();
            List<AMSAProgramSite> lstProgramSite = dbr.AMSAProgramSite.Where(m => m.AMSASite.id == departmentId).ToList();
            foreach(var a in lstProgramSite)
            {
                lstIdDepartment.Add(a.AMSAProgram.id);
            }
            //Get all programs and filter them
            foreach(int i in lstIdDepartment)
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