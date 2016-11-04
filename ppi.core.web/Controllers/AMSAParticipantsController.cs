using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Web.Models;
using PPI.Core.Web.Models.AmsaReports;
using PPI.Core.Web.Models.AmsaReports.ViewModel;
using System.IO;

namespace PPI.Core.Web.Controllers
{
    public class AMSAParticipantsController : Controller
    {
        public AMSAReportContext dbr { get; set; }
                                 
        public AMSAParticipantsController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAParticipants/
        public ActionResult Index()
        {
            var model = dbr.AMSAParticipant.ToList();
			return View(model);
        }

        // GET: /AMSAParticipants/Details/5
        public ActionResult Details(int id)
        {
            //Get data to show on view model
            var model = dbr.AMSAParticipant.First(m => m.Id == id);
            AMSAParticipantViewModel pvm = new AMSAParticipantViewModel();
            pvm.loadSelectedData(id);
            return View(model);
        }

        // GET: /AMSAParticipants/Create
        public ActionResult Create()
        {
            return View(new AMSAParticipantViewModel());
        }

        // POST: /AMSAParticipants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AMSAParticipantViewModel pvm)
        {
            ModelState.Remove("AMSAParticipant.AAMCNumber");
            ModelState.Remove("AMSAParticipant.AMSACode");
            ModelState.Remove("AMSAParticipant.AMSA_Password");
            if(pvm.AMSAParticipant.AMSACode != null)
                pvm.checkAMSACode(ModelState);
            pvm.checkIfUserAlreadyAssignedtoEvent(ModelState);
            if (ModelState.IsValid)
            {
                //db.AMSAParticipant.Add(aMSAParticipant);
                //db.SaveChanges();
                pvm.saveNewParticipant();
                return RedirectToAction("Index");
            }
            return View(pvm);
        }

        // GET: /AMSAParticipants/Edit/5
        public ActionResult Edit(int id)
        {
            AMSAParticipantViewModel pvm = new AMSAParticipantViewModel();
            pvm.loadSelectedData(id);
            return View(pvm);
        }

        // POST: /AMSAParticipants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AMSAParticipantViewModel pvm)
        {
            pvm.AMSAParticipant.AMSA_Password = "";
            ModelState.Remove("AMSAParticipant.AMSA_Password");
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAParticipant).State = EntityState.Modified;
                //db.SaveChanges();
                pvm.saveChanges();
                return RedirectToAction("Index");
            }
            return View(pvm);
        }

        // GET: /AMSAParticipants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAParticipant aMSAParticipant = db.AMSAParticipant.Find(id);
			var model = dbr.AMSAParticipant.First(m => m.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAParticipants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var model = dbr.AMSAParticipant.First(m => m.Id == id);
		    dbr.AMSAParticipant.Remove(model);
            dbr.SaveChanges();	
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult getAmmountOfCodes(int eventId)
        {
            try { 
                AMSAReportContext dbr = new AMSAReportContext();
                List<AMSACode> lstCodes = dbr.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == eventId).ToList();
                int ammountOfCodes = lstCodes.Count();
                dbr.Dispose();
                return Json(new
                {
                    error = false,
                    ammount = ammountOfCodes,
                },
                JsonRequestBehavior.AllowGet);
            }
            catch{
                return Json(new
                {
                    error = true,
                    ammount = 0,
                }, 
                JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View(new AMSAParticipantUploadViewModel());
        }

        [HttpPost]
        public ActionResult Upload(AMSAParticipantUploadViewModel pvm)
        {   
            
            ReportUtilities.checkUploadCSV(Request, ModelState);
            if (ModelState.IsValid)
            {
                pvm.PerformUserInsertionts(Request, ModelState);
                if (ModelState.IsValid)
                {
                    return View("Index", dbr.AMSAParticipant.ToList()); //Retun from when file is actually correct
                }
                else return View(pvm);
            }
            return View(pvm);
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
