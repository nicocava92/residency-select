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

namespace PPI.Core.Web.Controllers
{
    public class AMSACodesController : Controller
    {
        public AMSAReportContext dbr { get; set; }

        //Replace in Scafolding file
        //private AMSAReportContext db = new AMSAReportContext();

        public AMSACodesController()
        {
            dbr = new AMSAReportContext();
        }

        // GET: /AMSACodes/
        public ActionResult Index()
        {
            var model = dbr.AMSACodes.AsQueryable();
			return View(model);
        }

        // GET: /AMSACodes/Details/5
        public ActionResult Details(int id)
        {
            //AMSACode aMSACode = db.AMSACodes.Find(id);
			var model = dbr.AMSACodes.First(m => m.Id == id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        // GET: /AMSACodes/Create
        public ActionResult Create()
        {
            return View(new AMSACodeViewModel());
        }

        // POST: /AMSACodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AMSACodeViewModel acvm)
        {
            if(acvm.idSelectedEvent != 0)
            {
                ModelState.Remove("AMSACode.AMSAEvent");
            }
            acvm.checkIfCodeExists(ModelState);
            if (ModelState.IsValid)
            {
                acvm.saveNewCode();
                return RedirectToAction("Index");
            }

            return View(acvm);
        }

        [HttpGet]
        public ActionResult Upload()
        {
            return View(new AMSACodeUploadViewModel());
        }

        [HttpPost]
        public ActionResult Upload(AMSACodeUploadViewModel cvm)
        {
            ReportUtilities.checkUploadCSV(Request, ModelState);
            if (ModelState.IsValid)
            {
                cvm.PerformCodeInsertions(Request, ModelState);
                if (ModelState.IsValid)
                {
                    return View("Index", dbr.AMSACodes.ToList()); //Retun from when file is actually correct
                }
                else return View(cvm);
            }
            return View(cvm);
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
