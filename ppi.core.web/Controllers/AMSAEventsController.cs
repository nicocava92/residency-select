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
            return View(new AMSAEventViewModel());
        }

        // POST: /AMSAEvents/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AMSAEventViewModel ae)
        {
            if (ModelState.IsValid) //Need to change this because all information will not be present because of pagination
            {
                //db.AMSAEvent.Add(aMSAEvent);
                //db.SaveChanges();

                /************
                Instaed of creating right away we should send them to a page where they can view what the entered, give them the opportunity
                to come back or finish entering the Event
                ***************/

                
                return RedirectToAction("CreateInsertDates",ae);
            }

            return View();
        }

        //Get page that shows date insertion for the different values related to Amsa Event
        [HttpGet]
        public ActionResult CreateInsertDates(AMSAEventViewModel ae) {
            return View(ae);
        }
        //After dates are inserted show preview craete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInsertDatesPost(AMSAEventViewModel ae) {

            return View("PreviewCreate",ae);
        }

        //Get page that shows a preview of the information that will be stored for the AMSA Event at hand (if accepted stores)
        [HttpGet]
        public ActionResult PreviewCreate(AMSAEventViewModel ae)
        {
            return View(ae);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Stores the event in the database
        public ActionResult FinishCreatingEvent(AMSAEventViewModel ae)
        {
            //Check all of it for errors, if no errors are found then store the Event in the db
            if (ModelState.IsValid)
            {
                ae.saveNewEvent();
                return RedirectToAction("Index");
            }
            return View(ae);
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
			var model = dbr.AMSAEvent.First(m => m.id == id);
            AMSAEventViewModel ae = new AMSAEventViewModel();
            ae.AMSAEvent = model;
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
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAEvent).State = EntityState.Modified;
                //db.SaveChanges();
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
    }
}
