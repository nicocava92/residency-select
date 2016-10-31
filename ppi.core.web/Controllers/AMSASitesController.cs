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
using PPI.Core.Web.Models.AmsaReports.Event.ViewModel;

namespace PPI.Core.Web.Controllers
{
    public class AMSASitesController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        public AMSASitesController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSASites/
        public ActionResult Index()
        {
            AMSAReportContext dbo = new AMSAReportContext();
            var model = dbo.AMSASite.ToList();
			return View(model);
        }

        // GET: /AMSASites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSASite aMSASite = db.AMSASite.Find(id);
			var model = dbr.AMSASite.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSASites/Create
        public ActionResult Create()
        {
            return View(new AMSASiteViewModel());
        }

        // POST: /AMSASites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?Linkid=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AMSASiteViewModel asvm)
        {
            if (ModelState.IsValid)
            {
                asvm.saveNewSite();
                return RedirectToAction("Index");
            }

            return View(asvm);
        }

        // GET: /AMSASites/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSASite aMSASite = db.AMSASite.Find(id);
            var model = new AMSASiteViewModel();
            model.loadSelectedData(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSASites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?Linkid=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AMSASiteViewModel asvm)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSASite).State = EntityState.Modified;
                asvm.saveChanges();
				return RedirectToAction("Index");
            }
            return View(asvm);
        }

        // GET: /AMSASites/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSASite aMSASite = db.AMSASite.Find(id);
			var model = dbr.AMSASite.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSASites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSASite aMSASite = db.AMSASite.Find(id);
           // db.AMSASite.Remove(aMSASite);
           // db.SaveChanges();
		   var model = dbr.AMSASite.First(m => m.id == id);
		   dbr.AMSASite.Remove(model);	
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
