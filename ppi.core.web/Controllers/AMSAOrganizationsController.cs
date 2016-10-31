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
    public class AMSAOrganizationsController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSAOrganizationsController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSAOrganizations/
        public ActionResult Index()
        {
            var model = dbr.AMSAOrganization.AsQueryable();
			return View(model);
        }

        // GET: /AMSAOrganizations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAOrganization aMSAOrganization = db.AMSAOrganization.Find(id);
			var model = dbr.AMSAOrganization.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAOrganizations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSAOrganizations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] AMSAOrganization aMSAOrganization)
        {
            if (ModelState.IsValid)
            {
                //db.AMSAOrganization.Add(aMSAOrganization);
                //db.SaveChanges();
				 dbr.AMSAOrganization.Add(aMSAOrganization);
				 dbr.SaveChanges();		
                return RedirectToAction("Index");
            }

            return View(aMSAOrganization);
        }

        // GET: /AMSAOrganizations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAOrganization aMSAOrganization = db.AMSAOrganization.Find(id);
			var model = dbr.AMSAOrganization.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAOrganizations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] AMSAOrganization aMSAOrganization)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAOrganization).State = EntityState.Modified;
                //db.SaveChanges();
                AMSAOrganization ao = dbr.AMSAOrganization.Find(aMSAOrganization.id);
                ao.Name = aMSAOrganization.Name;
				dbr.SaveChanges();	
				return RedirectToAction("Index");
            }
            return View(aMSAOrganization);
        }

        // GET: /AMSAOrganizations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSAOrganization aMSAOrganization = db.AMSAOrganization.Find(id);
			var model = dbr.AMSAOrganization.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSAOrganizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSAOrganization aMSAOrganization = db.AMSAOrganization.Find(id);
           // db.AMSAOrganization.Remove(aMSAOrganization);
           // db.SaveChanges();
		   var model = dbr.AMSAOrganization.First(m => m.id == id);
            dbr.AMSAOrganization.Remove(model);
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
