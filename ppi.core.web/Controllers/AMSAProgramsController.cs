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
    public class AMSAProgramsController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSAProgramsController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAPrograms/
        public ActionResult Index()
        {
            var model = dbr.AMSAProgram.AsQueryable();
			return View(model);
        }

        // GET: /AMSAPrograms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAProgram aMSAProgram = db.AMSAProgram.Find(id);
			var model = dbr.AMSAProgram.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAPrograms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSAPrograms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] AMSAProgram aMSAProgram)
        {
            if (ModelState.IsValid)
            {
                //db.AMSAProgram.Add(aMSAProgram);
                //db.SaveChanges();
				dbr.AMSAProgram.Add(aMSAProgram);
                dbr.SaveChanges();	
                return RedirectToAction("Index");
            }

            return View(aMSAProgram);
        }

        // GET: /AMSAPrograms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAProgram aMSAProgram = db.AMSAProgram.Find(id);
			var model = dbr.AMSAProgram.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAPrograms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] AMSAProgram aMSAProgram)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAProgram).State = EntityState.Modified;
                //db.SaveChanges();
                AMSAProgram ap = dbr.AMSAProgram.Find(aMSAProgram.id);
                ap.Name = aMSAProgram.Name;
				dbr.SaveChanges();	
				return RedirectToAction("Index");
            }
            return View(aMSAProgram);
        }

        // GET: /AMSAPrograms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAProgram aMSAProgram = db.AMSAProgram.Find(id);
			var model = dbr.AMSAProgram.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAPrograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSAProgram aMSAProgram = db.AMSAProgram.Find(id);
           // db.AMSAProgram.Remove(aMSAProgram);
           // db.SaveChanges();
		   var model = dbr.AMSAProgram.First(m => m.id == id);
		   dbr.AMSAProgram.Remove(model);	
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
