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
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSAParticipant aMSAParticipant = db.AMSAParticipant.Find(id);
			var model = dbr.AMSAParticipant.First(m => m.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSAParticipants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSAParticipants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,FirstName,LastName,PrimaryEmail,Gender,Title,AMSACode")] AMSAParticipant aMSAParticipant)
        {
            if (ModelState.IsValid)
            {
                //db.AMSAParticipant.Add(aMSAParticipant);
                //db.SaveChanges();
				dbr.AMSAParticipant.Add(aMSAParticipant);
                dbr.SaveChanges();		
                return RedirectToAction("Index");
            }

            return View(aMSAParticipant);
        }

        // GET: /AMSAParticipants/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: /AMSAParticipants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,FirstName,LastName,PrimaryEmail,Gender,Title,AMSACode")] AMSAParticipant aMSAParticipant)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSAParticipant).State = EntityState.Modified;
                //db.SaveChanges();
                AMSAParticipant p = dbr.AMSAParticipant.Find(aMSAParticipant.Id);
                p.FirstName = aMSAParticipant.FirstName;
                p.LastName = aMSAParticipant.LastName;
                p.Gender = aMSAParticipant.Gender;
                p.PrimaryEmail = aMSAParticipant.PrimaryEmail;
                p.Title = aMSAParticipant.Title;
                p.AMSACode = aMSAParticipant.AMSACode;
                dbr.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(aMSAParticipant);
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
