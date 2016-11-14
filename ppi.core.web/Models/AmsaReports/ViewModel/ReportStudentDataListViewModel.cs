using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class ReportStudentDataListViewModel
    {
        public List<AmsaReportStudentData> LstStudentData { get; set; }

        public SelectList Events { get; set; }
        public List<AMSAEvent> LstEvents { get; set; }
        public int idSelectedEvent { get; set; }

        public ReportStudentDataListViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            loadEvents(dbr);
            LstStudentData = dbr.lstStudentsForReport.ToList();
            dbr.Dispose();
        }

        private void loadEvents(AMSAReportContext dbr)
        {
            LstEvents = dbr.AMSAEvent.ToList();
            //Add a new event to the top of the list to select all events, id = 0
            LstEvents.Insert(0, new AMSAEvent { id = 0, Name = "Show All" });
            Events = new SelectList(LstEvents, "id", "Name");
        }
    }
}