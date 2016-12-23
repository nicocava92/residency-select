using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Web.Models.AmsaReports.ViewModel;
using PPI.Core.Web.Models.AmsaReports;
using PPI.Core.Web.Models;
using System.Data.Entity;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;

namespace PPI.Core.Web.Controllers
{
    public class AMSABillingController : Controller
    {


        public ActionResult Index()
        {
            var model = new AMSABillingViewModel();
            model.LstAMSAReports = new List<AmsaReportStudentData>();
            model.LlstAMSAEvents = new List<AMSAEvent>();
            return View(model);
        }
        [HttpPost]
        public ActionResult Review(string datepickerDateRange)
        {
            AMSAReportContext db = new AMSAReportContext();
            try
            {
                
                var model = new AMSABillingViewModel();
                var startDate = DateTime.Parse(datepickerDateRange.Substring(0, datepickerDateRange.IndexOf("-") - 1));
                var endDate = DateTime.Parse(datepickerDateRange.Substring(datepickerDateRange.IndexOf("-") + 1));
                model.LlstAMSAEvents = db.AMSAEvent.Where(m => DbFunctions.TruncateTime(m.CreateDate) >= startDate && DbFunctions.TruncateTime(m.CreateDate) <= endDate && m.Billable == true).ToList();
                //UnitOfWork.IEventRepository.AsQueryable().Where(m => DbFunctions.TruncateTime(m.CreateDate) >= startDate && DbFunctions.TruncateTime(m.CreateDate) <= endDate && m.Billable == true).ToList();
                //Get events that we will be searching for in the reports
                model.LstAMSAReports = db.lstStudentsForReport
                    .Where(m => DbFunctions.TruncateTime(m.AMSAEvent.StartDate) >= startDate && DbFunctions.TruncateTime(m.AMSAEvent.StartDate) <= endDate)
                    .OrderBy(m => m.Id).ToList();
                //var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));            
                //db.dispose();
                return View("Index", model);
            }
            catch
            {
                //db.dispose();
                return View("Index");
            }
        }
        public ActionResult Participants(int eventId)
        {
            AMSAReportContext db = new AMSAReportContext();
            var model = db.AMSAParticipant.Where(m => m.AMSAEvent.id == eventId);
            //db.dispose();
            ViewBag.AMSAEventid = eventId;
            return PartialView("_PartialParticipantsAMSA", model);
        }
        
        public ActionResult ExportParticipants(int eventId)
        {
            AMSAReportContext db = new AMSAReportContext();
            string exportName = "Participants" + ".csv";
            CsvWriter writer;
            var streamoutput = new MemoryStream();
            var sw = new StreamWriter(streamoutput);
            var PeopleList = db.AMSAParticipant.Where(m => m.AMSAEvent.id == eventId);
            if (PeopleList.Count() > 0)
            {
                var CsvConfig = new CsvConfiguration();
                writer = new CsvWriter(sw, CsvConfig);
                IEnumerable<AMSAParticipant> records = PeopleList.ToList();
                //Might need to optimize this code in the future to remove tihs step
                List<AMSAParticipantForCSV> lstRecordsForCSV = new List<AMSAParticipantForCSV>();
                foreach (AMSAParticipant p in records)
                {
                    //Use aux class to convert the participant and only download the requested data
                    AMSAParticipantForCSV auxP = new AMSAParticipantForCSV
                    {
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        PrimaryEmail = p.PrimaryEmail,
                        Gender = p.Gender,
                        Title = p.Title,
                        AMSACode = p.AMSACode,
                        AAMCNumber = p.AAMCNumber,
                    };
                    lstRecordsForCSV.Add(auxP);
                }
                writer.WriteRecords(lstRecordsForCSV);
                sw.Flush();
            }
            streamoutput.Seek(0, SeekOrigin.Begin);
            //db.dispose();
            return File(streamoutput, "text/csv", exportName);
        }
        //public ActionResult ExportReports(int eventId)
        //{
        //    string exportName = "Reports" + ".csv";
        //    CsvWriter writer;
        //    var streamoutput = new MemoryStream();
        //    var sw = new StreamWriter(streamoutput);
        //    var personreports = UnitOfWork.IPersonPracticeReportRepository.AsQueryable().Where(m => m.EventId == eventId)
        //        .GroupBy(m => m.PracticeReportId)
        //        //.Select(n => new {reportId = n.Key, report = n.Select(y => y.PracticeReport), people = n.Select(x => x.Person)});
        //        .Select(n => new ReportsRun() { Report = n.Select(m => m.PracticeReport).FirstOrDefault(), Participants = n.Select(m => m.Person).Distinct() });
        //    if (personreports.Count() > 0)
        //    {
        //        var CsvConfig = new CsvConfiguration();
        //        writer = new CsvWriter(sw, CsvConfig);
        //        writer.WriteRecords(personreports.ToList());
        //        sw.Flush();
        //    }
        //    streamoutput.Seek(0, SeekOrigin.Begin);
        //    return File(streamoutput, "text/csv", exportName);
        //}


    }
}