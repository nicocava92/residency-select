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
    public class AMSASurveyTypesController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSASurveyTypesController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSASurveyTypes/
        public ActionResult Index()
           
        {
            
            var model = dbr.AMSASurveySiteType.AsQueryable();
			return View(model);
        }

        // GET: /AMSASurveyTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
			//replace scafolding
            //AMSASurveyType aMSASurveyType = db.AMSASurveySiteType.Find(id);
			var model = dbr.AMSASurveySiteType.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSASurveyTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /AMSASurveyTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,Name")] AMSASurveyType aMSASurveyType)
        {
            if (ModelState.IsValid)
            {
                //db.AMSASurveySiteType.Add(aMSASurveyType);
                //db.SaveChanges();
				 dbr.AMSASurveySiteType.Add(aMSASurveyType);
				 dbr.SaveChanges();		
                return RedirectToAction("Index");
            }

            return View(aMSASurveyType);
        }

        // GET: /AMSASurveyTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSASurveyType aMSASurveyType = db.AMSASurveySiteType.Find(id);
			var model = dbr.AMSASurveySiteType.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSASurveyTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,Name")] AMSASurveyType aMSASurveyType)
        {
            if (ModelState.IsValid)
            {
                // db.Entry(aMSASurveyType).State = EntityState.Modified;
                //db.SaveChanges();
                AMSASurveyType at = dbr.AMSASurveySiteType.Find(aMSASurveyType.id);
                at.Name = aMSASurveyType.Name;
                dbr.SaveChanges();
				return RedirectToAction("Index");
            }
            return View(aMSASurveyType);
        }

        // GET: /AMSASurveyTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //AMSASurveyType aMSASurveyType = db.AMSASurveySiteType.Find(id);
			var model = dbr.AMSASurveySiteType.First(m => m.id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // POST: /AMSASurveyTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
           // AMSASurveyType aMSASurveyType = db.AMSASurveySiteType.Find(id);
           // db.AMSASurveySiteType.Remove(aMSASurveyType);
           // db.SaveChanges();
		   var model = dbr.AMSASurveySiteType.First(m => m.id == id);
		   dbr.AMSASurveySiteType.Remove(model);
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
