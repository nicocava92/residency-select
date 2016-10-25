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
        public int idSelectedEventType { get; set; }
        public SelectList AMSAEventStatus { get; set; }
        public int idSelectedEventStatus { get; set; }
        public SelectList AMSAOrganization { get; set; }
        public int idSelectedOrganization { get; set; }
        public SelectList AMSAProgramSite { get; set; }
        public int idSelectedProgramSite { get; set; }
        public SelectList AMSASurveyType { get; set; }
        public int idSelectedSurveyType { get; set; }
        public SelectList AMSANotBillableReason { get; set; }
        public int idSelectedNotBillableReason { get; set; }

        public AMAEventViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEventType = new SelectList(dbr.AMSAEventType.ToList(), "id", "Name");
            AMSAEventStatus = new SelectList(dbr.AMSAEventStatus.ToList(), "id", "Name");
            AMSAProgramSite = new SelectList(dbr.AMSAProgramSite.ToList(), "id", "Name");
            AMSASurveyType = new SelectList(dbr.AMSASurveySiteType.ToList(), "id", "Name");
            dbr.Dispose();
        }

        public void loadSelectedData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent ae = dbr.AMSAEvent.Find(id);
            idSelectedEventType = ae.AMSAEventType.id;
            idSelectedEventStatus = ae.AMSAEventStatus.id;
            idSelectedOrganization = ae.AMSAOrganization.id;
            idSelectedProgramSite = ae.AMSAProgramSite.id;
            idSelectedSurveyType = ae.AMSASurveyType.id;
            idSelectedNotBillableReason = ae.AMSANotBillableReason.id;
            dbr.Dispose();
        }
        public void saveChanges()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent ae = dbr.AMSAEvent.Find(this.AMSAEvent.id);
            //Store Fk to classes it points to now
            ae.AMSAEventType = dbr.AMSAEventType.Find(this.idSelectedEventType);
            ae.AMSAEventStatus = dbr.AMSAEventStatus.Find(this.idSelectedEventStatus);
            ae.AMSAOrganization = dbr.AMSAOrganization.Find(this.idSelectedOrganization);
            ae.AMSAProgramSite = dbr.AMSAProgramSite.Find(this.idSelectedProgramSite);
            ae.AMSASurveyType = dbr.AMSASurveySiteType.Find(this.idSelectedSurveyType);
            ae.AMSANotBillableReason = dbr.AMSANotBillableReason.Find(this.idSelectedNotBillableReason);
            //Store changes to attributes
            ae.Name = this.AMSAEvent.Name;
            ae.Description = this.AMSAEvent.Description;
            ae.StartDate = this.AMSAEvent.StartDate;
            ae.EndDate = this.AMSAEvent.EndDate;
            ae.ReviewDate = this.AMSAEvent.ReviewDate;
            ae.CreateDate = this.AMSAEvent.CreateDate;
            ae.CompositeNeedByDate = this.AMSAEvent.CompositeNeedByDate;
            ae.JetNeedByDate = this.AMSAEvent.JetNeedByDate;
            ae.Placement = this.AMSAEvent.Placement;
            ae.Billable = this.AMSAEvent.Billable;
            ae.TotalNumberOfParticipants = this.AMSAEvent.TotalNumberOfParticipants;
            ae.defaultEmailAddress = this.AMSAEvent.defaultEmailAddress;
            ae.OrderBy = this.AMSAEvent.OrderBy;
            ae.JetRequired = this.AMSAEvent.JetRequired;
            ae.CompositeRequired = this.AMSAEvent.CompositeRequired;

            dbr.SaveChanges();
            dbr.Dispose();
        }
        
        public void saveNewsite()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent ae = new AMSAEvent();
            ae.AMSAEventType = dbr.AMSAEventType.Find(this.idSelectedEventType);
            ae.AMSAEventStatus = dbr.AMSAEventStatus.Find(this.idSelectedEventStatus);
            ae.AMSAOrganization = dbr.AMSAOrganization.Find(this.idSelectedOrganization);
            ae.AMSAProgramSite = dbr.AMSAProgramSite.Find(this.idSelectedProgramSite);
            ae.AMSASurveyType = dbr.AMSASurveySiteType.Find(this.idSelectedSurveyType);
            ae.AMSANotBillableReason = dbr.AMSANotBillableReason.Find(this.idSelectedNotBillableReason);
            //Store changes to attributes
            ae.Name = this.AMSAEvent.Name;
            ae.Description = this.AMSAEvent.Description;
            ae.StartDate = this.AMSAEvent.StartDate;
            ae.EndDate = this.AMSAEvent.EndDate;
            ae.ReviewDate = this.AMSAEvent.ReviewDate;
            ae.CreateDate = this.AMSAEvent.CreateDate;
            ae.CompositeNeedByDate = this.AMSAEvent.CompositeNeedByDate;
            ae.JetNeedByDate = this.AMSAEvent.JetNeedByDate;
            ae.Placement = this.AMSAEvent.Placement;
            ae.Billable = this.AMSAEvent.Billable;
            ae.TotalNumberOfParticipants = this.AMSAEvent.TotalNumberOfParticipants;
            ae.defaultEmailAddress = this.AMSAEvent.defaultEmailAddress;
            ae.OrderBy = this.AMSAEvent.OrderBy;
            ae.JetRequired = this.AMSAEvent.JetRequired;
            ae.CompositeRequired = this.AMSAEvent.CompositeRequired;
            dbr.AMSAEvent.Add(ae);
            dbr.Dispose();
        }
    }
}