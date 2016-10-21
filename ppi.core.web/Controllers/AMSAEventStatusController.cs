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

namespace PPI.Core.Web.Controllers
{
    public class AMSAEventStatusController : Controller
    {
        //Replace in Scafolding file
        //private ApplicationDbContext db = new ApplicationDbContext();
        public AMSAReportContext dbr { get; set; }

        public AMSAEventStatusController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAEventStatus/
        public ActionResult Index()
        {
            return View(dbr.AMSAEventStatus.ToList());
        }

        // GET: /AMSAEventStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAEventStatus aMSAEventStatus = db.AMSAEventStatus.Find(id);
			var model = dbr.AMSAEventStatus.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAEventStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSAEventStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] AMSAEventStatus aMSAEventStatus)
        {
            if (ModelState.IsValid)
            {
                //db.AMSAEventStatus.Add(aMSAEventStatus);
                //db.SaveChanges();
				 dbr.AMSAEventStatus.Add(aMSAEventStatus);
				 dbr.SaveChanges();		
                return RedirectToAction("Index");
            }

            return View(aMSAEventStatus);
        }

        // GET: /AMSAEventStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAEventStatus aMSAEventStatus = db.AMSAEventStatus.Find(id);
			var model = dbr.AMSAEventStatus.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAEventStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] AMSAEventStatus aMSAEventStatus)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAEventStatus).State = EntityState.Modified;
                //db.SaveChanges();
                AMSAEventStatus aes = dbr.AMSAEventStatus.Find(aMSAEventStatus.id);
                aes.Name = aMSAEventStatus.Name;
                dbr.SaveChanges();	
				return RedirectToAction("Index");
            }
            return View(aMSAEventStatus);
        }

        // GET: /AMSAEventStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAEventStatus aMSAEventStatus = db.AMSAEventStatus.Find(id);
			var model = dbr.AMSAEventStatus.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAEventStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSAEventStatus aMSAEventStatus = db.AMSAEventStatus.Find(id);
           // db.AMSAEventStatus.Remove(aMSAEventStatus);
           // db.SaveChanges();
		   var model = dbr.AMSAEventStatus.First(m => m.id == id);
            dbr.AMSAEventStatus.Remove(model);
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
