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
using System.IO;

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

        [Authorize(Roles = "AMSASiteCoordinator,Admin")]
        public ActionResult Index()
        {
            ParticipantListViewModel pvm = new ParticipantListViewModel();
            return View(pvm);
        }

        // GET: /AMSAParticipants/Details/5
        [Authorize(Roles = "AMSASiteCoordinator,Admin")]
        public ActionResult Details(int id)
        {
            //Get data to show on view model
            var model = dbr.AMSAParticipant.First(m => m.Id == id);
            AMSAParticipantViewModel pvm = new AMSAParticipantViewModel();
            pvm.loadSelectedData(id);
            return View(model);
        }

        // GET: /AMSAParticipants/Create
        [Authorize(Roles = "AMSASiteCoordinator,Admin")]
        public ActionResult Create()
        {
            return View(new AMSAParticipantViewModel());
        }

        // POST: /AMSAParticipants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "AMSASiteCoordinator,Admin")]
        public ActionResult Create(AMSAParticipantViewModel pvm)
        {
            ModelState["AMSAParticipant.AAMCNumber"].Errors.Clear() ;
            ModelState["AMSAParticipant.AMSACode"].Errors.Clear();
            ModelState["AMSAParticipant.AMSA_Password"].Errors.Clear();
            if (pvm.AMSAParticipant.AMSACode != null)
                pvm.checkAMSACode(ModelState);
            pvm.checkIfUserAlreadyAssignedtoEvent(ModelState);
            if (ModelState.IsValid)
            {
                //db.AMSAParticipant.Add(aMSAParticipant);
                //db.SaveChanges();
                pvm.saveNewParticipant();
                return RedirectToAction("Index");
            }
            return View(pvm);
        }

        // GET: /AMSAParticipants/Edit/5
        public ActionResult Edit(int id)
        {
            AMSAParticipantViewModel pvm = new AMSAParticipantViewModel();
            pvm.loadSelectedData(id);
            return View(pvm);
        }

        // POST: /AMSAParticipants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AMSAParticipantViewModel pvm)
        {
            ModelState.Remove("AMSAParticipant.AMSA_Password");
            ModelState.Remove("AMSAParticipant.Title");
            ModelState.Remove("AMSAParticipant.AAMCNumber");
            ModelState.Remove("AMSAParticipant.AMSACode");
            ModelState.Remove("AMSAParticipant.PrimaryEmail");
            //Check if the e-mail is unique or if exists for another Participant already
            AMSAParticipant p = dbr.AMSAParticipant.Where(m => m.PrimaryEmail.ToUpper().Equals(pvm.AMSAParticipant.PrimaryEmail.ToUpper())).FirstOrDefault();
            if(p != null)
            {
                ModelState.AddModelError("AMSAParticipantEmail", "There is another participant already registered with this e-mail address, please enter another e-mail address.");
            }

            if (ModelState.IsValid)
            {
                // db.Entry(aMSAParticipant).State = EntityState.Modified;
                //db.SaveChanges();
                pvm.saveChanges();
                return RedirectToAction("Index");
            }
            return View(pvm);
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

        [HttpGet]
        public ActionResult getAmmountOfCodes(int eventId)
        {
            try {
                AMSAReportContext dbr = new AMSAReportContext();
                List<AMSACode> lstCodes = dbr.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == eventId).ToList();
                int ammountOfCodes = lstCodes.Count();
                dbr.Dispose();
                return Json(new
                {
                    error = false,
                    ammount = ammountOfCodes,
                },
                JsonRequestBehavior.AllowGet);
            }
            catch {
                return Json(new
                {
                    error = true,
                    ammount = 0,
                },
                JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Authorize(Roles = "AMSASiteCoordinator,Admin")]
        public ActionResult Upload(int? id)
        {
            AMSAParticipantUploadViewModel apvm = new AMSAParticipantUploadViewModel();
            int eventId = id ?? 0;
            if(eventId > 0)
            {
                apvm.idSelectedEvent = eventId;
            }
            return View(apvm);
        }

        [HttpPost]
        [Authorize(Roles = "AMSASiteCoordinator,Admin")]
        public ActionResult Upload(AMSAParticipantUploadViewModel pvm)
        {

            ReportUtilities.checkUploadCSVandExcel(Request, ModelState);
            if (ModelState.IsValid)
            {
                pvm.PerformUserInsertionts(Request, ModelState);
                if (ModelState.IsValid)
                {
                    return View("Index", new ParticipantListViewModel()); //Retun from when file is actually correct
                }
                else return View(pvm);
            }
            return View(pvm);
        }

        //Return participant list by the id that is selected
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetParticipantsByEvent(ParticipantListViewModel pvm)
        {
            try {
                List<AMSAParticipant> participantsInEvent = new List<AMSAParticipant>();
                //if the id != to 0 then filter
                if (pvm.idSelectedEvent != 0) {
                    //Get the listing of information that needs to be shown on the view
                    participantsInEvent = dbr.AMSAParticipant.Where(r => r.AMSAEvent.id == pvm.idSelectedEvent).ToList();
                }
                else
                {
                    //If it is == 0 then what we are need to show is all of the participants
                    participantsInEvent = dbr.AMSAParticipant.ToList();
                }
                pvm.LstParticipants = participantsInEvent;
                return View("Index", pvm);
            }
            catch
            {
                return View("Index", pvm);
            }
        }

        //Used to get events by id from dashboard
        public ActionResult GetParticipantsByEvent(int? eventId)
        {
            ParticipantListViewModel pvm = new ParticipantListViewModel();
            pvm.idSelectedEvent = eventId ?? 0;
            try
            {
                List<AMSAParticipant> participantsInEvent = new List<AMSAParticipant>();
                //if the id != to 0 then filter
                if (eventId != 0 && eventId != -1 && eventId != null)
                {
                    //Get the listing of information that needs to be shown on the view
                    participantsInEvent = dbr.AMSAParticipant.Where(r => r.AMSAEvent.id == eventId).ToList();
                }
                else
                {
                    //If it is == 0 then what we are need to show is all of the participants
                    participantsInEvent = dbr.AMSAParticipant.ToList();
                }
                
                pvm.LstParticipants = participantsInEvent;
                return View("Index", pvm);
            }
            catch
            {
                return View("Index", pvm);
            }
        }

        //Receives a search string from view and searches for participants who have the inserted values
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetParticipantsBySearch(ParticipantListViewModel pvm)
        {
            try {
                string s = pvm.Search.ToUpper();
                //Serach participants by Last Name, Name, Email, AAMC Number and or AMSACode
                List<AMSAParticipant> participantsFoundInSearch = dbr.AMSAParticipant.Where(r => r.LastName.ToUpper().Equals(s) || r.FirstName.ToUpper().Equals(s) || r.PrimaryEmail.ToUpper().Equals(s) || r.AAMCNumber.ToUpper().Equals(s) || r.AMSACode.ToUpper().Equals(s)).ToList();
                ParticipantListViewModel retPvm = new ParticipantListViewModel();
                retPvm.LstParticipants = participantsFoundInSearch;
                return View("Index", retPvm);
            }
            catch
            {
                return View("Index", pvm);
            }
        }

        //Show participant profile
        [HttpGet]
        public ActionResult Profile(int id)
        {
            return View(new AMSAParticipantProfileViewModel(id));
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
