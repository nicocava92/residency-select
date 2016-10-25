using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Web.Models.AmsaReports.Event;

namespace PPI.Core.Web.Models.AmsaReports.Event.ViewModel
{
    public class AMSAEventViewModel
    {
        public AMSAEvent AMSAEvent { get; set; }

        public SelectList AMSAEventType { get; set; }
        public int idSelectedEventType { get; set; }
        public SelectList AMSAEventStatus { get; set; }
        public int idSelectedEventStatus { get; set; }
        public SelectList AMSAOrganization { get; set; }
        public int idSelectedOrganization { get; set; }
        public SelectList AMSASurveyType { get; set; }
        public int idSelectedSurveyType { get; set; }
        public SelectList AMSANotBillableReason { get; set; }
        public int idSelectedNotBillableReason { get; set; }
        public SelectList AMSAProgram { get; set; }
        public int idSelectedProgram { get; set; }
        public SelectList AMSASite { get; set; }
        public int idSelectedSite { get; set; }

        //Select yes or no
        public SelectList YesNo { get; set; }
        public List<YesNo> ValuesYesNo { get; set; }

        


        public AMSAEventViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEventType = new SelectList(dbr.AMSAEventType.ToList(), "id", "Name");
            AMSAEventStatus = new SelectList(dbr.AMSAEventStatus.ToList(), "id", "Name");
            AMSASurveyType = new SelectList(dbr.AMSASurveySiteType.ToList(), "id", "Name");
            AMSANotBillableReason = new SelectList(dbr.AMSANotBillableReason.ToList(), "id", "Name");
            AMSAOrganization = new SelectList(dbr.AMSAOrganization.ToList(), "id", "Name");
            AMSASite = new SelectList(dbr.AMSASite.ToList(), "id", "Name");
            AMSAProgram = new SelectList(dbr.AMSAProgram.ToList(), "id", "Name");

            //Order by set
            this.AMSAEvent = new AMSAEvent();
            AMSAEvent.OrderBy = "perform";
            addValuesToYesNo();
            this.YesNo = new SelectList(this.ValuesYesNo, "value", "name");
            dbr.Dispose();
        }

        public void addValuesToYesNo()
        {
            this.ValuesYesNo = new List<YesNo>();
            this.ValuesYesNo.Add(new YesNo { name = "Yes", value = true });
            this.ValuesYesNo.Add(new YesNo { name = "No", value = false });
        }

        public void loadSelectedData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent ae = dbr.AMSAEvent.Find(id);
            idSelectedEventType = ae.AMSAEventType.id;
            idSelectedEventStatus = ae.AMSAEventStatus.id;
            idSelectedOrganization = ae.AMSAOrganization.id;
            idSelectedProgram = ae.AMSAProgram.id;
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
            ae.AMSAProgram = dbr.AMSAProgram.Find(this.idSelectedProgram);
            ae.AMSASurveyType = dbr.AMSASurveySiteType.Find(this.idSelectedSurveyType);
            ae.AMSANotBillableReason = dbr.AMSANotBillableReason.Find(this.idSelectedNotBillableReason);
            ae.AMSASite = dbr.AMSASite.Find(this.idSelectedSite);
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
        
        public void saveNewEvent()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent ae = this.AMSAEvent;
            ae.AMSAEventType = dbr.AMSAEventType.Find(this.idSelectedEventType);
            ae.AMSAEventStatus = dbr.AMSAEventStatus.Find(this.idSelectedEventStatus);
            ae.AMSAOrganization = dbr.AMSAOrganization.Find(this.idSelectedOrganization);
            ae.AMSAProgram = dbr.AMSAProgram.Find(this.idSelectedProgram);
            ae.AMSASurveyType = dbr.AMSASurveySiteType.Find(this.idSelectedSurveyType);
            ae.AMSANotBillableReason = dbr.AMSANotBillableReason.Find(this.idSelectedNotBillableReason);
            ae.AMSASite = dbr.AMSASite.Find(this.idSelectedSite);
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

        //Return possible departments to the view
        public SelectList getPossibleDepartment()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            dbr.Dispose();
            return new SelectList(dbr.AMSASite.Where(m => m.AMSAOrganization.id == this.idSelectedOrganization).ToList(),"id","Name");
        } 

        //Check if department (site) - speciality (program) exists before insertion
        //They should exist in the many to many list, if not user can either add them or correct their error.
        public bool departmentSpecialityExists()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            bool ret = false;
            List<AMSAProgramSite> aps = dbr.AMSAProgramSite.Where(m => m.AMSAProgram.id == idSelectedProgram && m.AMSASite.id == idSelectedSite).ToList();
            if (aps.Count > 0)
                ret = true;
            dbr.Dispose();
            return ret;
        }
    }
    public class YesNo
    {
        public string name { get; set; }
        public bool value { get; set; }
    }
}