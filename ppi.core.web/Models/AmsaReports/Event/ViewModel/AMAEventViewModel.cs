using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.Event.ViewModel
{
    public class AMAEventViewModel
    {
        public AMSAEvent AMSAEvent { get; set; }
        public SelectList AMSAEventType { get; set; }
        public SelectList AMSAEventStatus { get; set; }
        public SelectList AMSAProgramSite { get; set; }
        public SelectList AMSASurveyType { get; set; }

        public AMAEventViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEventType = new SelectList(dbr.AMSAEventType.ToList(), "id", "Name");
            AMSAEventStatus = new SelectList(dbr.AMSAEventStatus.ToList(), "id", "Name");
            AMSAProgramSite = new SelectList(dbr.AMSAProgramSite.ToList(), "id", "Name");
            AMSASurveyType = new SelectList(dbr.AMSASurveySiteType.ToList(), "id", "Name");
            dbr.Dispose();
        }
        
    }
}