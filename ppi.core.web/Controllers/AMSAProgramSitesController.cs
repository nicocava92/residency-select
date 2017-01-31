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
    public class AMSAProgramSitesController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSAProgramSitesController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAProgramSites/
        public ActionResult Index()
        {
            var model = dbr.AMSAProgramSite.ToList();
			return View(model);
        }

        // GET: /AMSAProgramSites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAProgramSite aMSAProgramSite = db.AMSAProgramSite.Find(id);
			var model = dbr.AMSAProgramSite.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAProgramSites/Create
        public ActionResult Create()
        {
            
            return View(new AMSAProgramSiteViewModel());
        }

        // POST: /AMSAProgramSites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?Linkid=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AMSAProgramSiteViewModel avm)
        {
            if(!(avm.idSelectedProgram > 0))
            {
                ModelState.AddModelError("AMSAProgram","An AMSA Program Site can't be inserted without a AMSAProgram");
            }
            if(!(avm.idSelectedSite > 0))
            {
                ModelState.AddModelError("AMSASite", "An AMSA Program Site can't be inserted without a AMSASite");
            }

            if (ModelState.IsValid)
            {
                //db.AMSAProgramSite.Add(aMSAProgramSite);
                //db.SaveChanges();
                avm.saveNewSite();
                return RedirectToAction("Index");
            }

            return View(avm);
        }

        // GET: /AMSAProgramSites/Edit/5
        public ActionResult Edit(int? id)
        {
           

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAProgramSite aMSAProgramSite = db.AMSAProgramSite.Find(id);
			var model = dbr.AMSAProgramSite.First(m => m.id == id);
            AMSAProgramSiteViewModel avm = new AMSAProgramSiteViewModel();
            avm.idSelectedProgram = model.AMSAProgram.id;
            avm.idSelectedSite = model.AMSASite.id;
            avm.AMSAProgramSite = dbr.AMSAProgramSite.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(avm);
        }

        // POST: /AMSAProgramSites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?Linkid=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AMSAProgramSiteViewModel avm)
        {
            if (!(avm.idSelectedProgram > 0))
            {
                ModelState.AddModelError("AMSAProgram", "An AMSA Program Site can't be inserted without a AMSAProgram");
            }
            if (!(avm.idSelectedSite > 0))
            {
                ModelState.AddModelError("AMSASite", "An AMSA Program Site can't be inserted without a AMSASite");
            }
            // db.Entry(aMSAProgramSite).State = EntityState.Modified;
            //db.SaveChanges();
            if (ModelState.IsValid) { 
                avm.saveChanges();
                return RedirectToAction("Index");
            }
            return View(avm);
        }

        // GET: /AMSAProgramSites/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAProgramSite aMSAProgramSite = db.AMSAProgramSite.Find(id);
			var model = dbr.AMSAProgramSite.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAProgramSites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSAProgramSite aMSAProgramSite = db.AMSAProgramSite.Find(id);
           // db.AMSAProgramSite.Remove(aMSAProgramSite);
           // db.SaveChanges();
		    var model = dbr.AMSAProgramSite.First(m => m.id == id);
		    dbr.AMSAProgramSite.Remove(model);
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
