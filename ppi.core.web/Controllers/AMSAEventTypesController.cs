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
    public class AMSAEventTypesController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSAEventTypesController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAEventTypes/
        public ActionResult Index()
        {
            var model = dbr.AMSAEventType.AsQueryable();
			return View(model);
        }

        // GET: /AMSAEventTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAEventType aMSAEventType = db.AMSAEventType.Find(id);
			var model = dbr.AMSAEventType.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAEventTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSAEventTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] AMSAEventType aMSAEventType)
        {
            if (ModelState.IsValid)
            {
                //db.AMSAEventType.Add(aMSAEventType);
                //db.SaveChanges();
				 dbr.AMSAEventType.Add(aMSAEventType);
				 dbr.SaveChanges();		
                return RedirectToAction("Index");
            }

            return View(aMSAEventType);
        }

        // GET: /AMSAEventTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAEventType aMSAEventType = db.AMSAEventType.Find(id);
			var model = dbr.AMSAEventType.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAEventTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] AMSAEventType aMSAEventType)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAEventType).State = EntityState.Modified;
                //db.SaveChanges();
                AMSAEventType avt = dbr.AMSAEventType.Find(aMSAEventType.id);
                avt.Name = aMSAEventType.Name;
				dbr.SaveChanges();	
				return RedirectToAction("Index");
            }
            return View(aMSAEventType);
        }

        // GET: /AMSAEventTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAEventType aMSAEventType = db.AMSAEventType.Find(id);
			var model = dbr.AMSAEventType.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAEventTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSAEventType aMSAEventType = db.AMSAEventType.Find(id);
           // db.AMSAEventType.Remove(aMSAEventType);
           // db.SaveChanges();
		   var model = dbr.AMSAEventType.First(m => m.id == id);
		   dbr.AMSAEventType.Remove(model);	
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
