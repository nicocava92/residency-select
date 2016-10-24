using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Web.Models;
using PPI.Core.Web.Models.AmsaReports.Event;

namespace PPI.Core.Web.Controllers
{
    public class AMSANotBillableReasonsController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSANotBillableReasonsController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSANotBillableReasons/
        public ActionResult Index()
        {
            
            var model = dbr.AMSANotBillableReason.AsQueryable();
			return View(model);
        }

        // GET: /AMSANotBillableReasons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSANotBillableReason aMSANotBillableReason = db.AMSANotBillableReasons.Find(id);
			var model = dbr.AMSANotBillableReason.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSANotBillableReasons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSANotBillableReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] AMSANotBillableReason aMSANotBillableReason)
        {
            if (ModelState.IsValid)
            {
                //db.AMSANotBillableReasons.Add(aMSANotBillableReason);
                //db.SaveChanges();
				dbr.AMSANotBillableReason.Add(aMSANotBillableReason);
                dbr.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(aMSANotBillableReason);
        }

        // GET: /AMSANotBillableReasons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSANotBillableReason aMSANotBillableReason = db.AMSANotBillableReasons.Find(id);
			var model = dbr.AMSANotBillableReason.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSANotBillableReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] AMSANotBillableReason aMSANotBillableReason)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSANotBillableReason).State = EntityState.Modified;
                //db.SaveChanges();
                AMSANotBillableReason billable = dbr.AMSANotBillableReason.Find(aMSANotBillableReason.id);
                billable.Name = aMSANotBillableReason.Name;
                dbr.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aMSANotBillableReason);
        }

        // GET: /AMSANotBillableReasons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSANotBillableReason aMSANotBillableReason = db.AMSANotBillableReasons.Find(id);
			var model = dbr.AMSANotBillableReason.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSANotBillableReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSANotBillableReason aMSANotBillableReason = db.AMSANotBillableReasons.Find(id);
           // db.AMSANotBillableReasons.Remove(aMSANotBillableReason);
           // db.SaveChanges();
		    var model = dbr.AMSANotBillableReason.First(m => m.id == id);
		    dbr.AMSANotBillableReason.Remove(model);
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
