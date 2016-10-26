﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PPI.Core.Web.Models.AmsaReports.Event;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Please select an Organization")]
        public int idSelectedOrganization { get; set; }
        public SelectList AMSASurveyType { get; set; }
        public int idSelectedSurveyType { get; set; }
        public SelectList AMSANotBillableReason { get; set; }
        public int idSelectedNotBillableReason { get; set; }
        public SelectList AMSAProgram { get; set; }
        [Required(ErrorMessage = "Please select a Program")]
        public int idSelectedProgram { get; set; }
        public SelectList AMSASite { get; set; }
        [Required(ErrorMessage = "Please select a Site")]
        public int idSelectedSite { get; set; }

        //Select yes or no
        public SelectList YesNo { get; set; }
        public List<YesNo> ValuesYesNo { get; set; }

        private List<AMSAOrganization> ListOfOrganization = new List<AMSAOrganization>();
        private List<AMSASite> ListOfAmsaSite = new List<AMSASite>();


        public AMSAEventViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEventType = new SelectList(dbr.AMSAEventType.ToList(), "id", "Name");
            AMSAEventStatus = new SelectList(dbr.AMSAEventStatus.ToList(), "id", "Name");
            AMSASurveyType = new SelectList(dbr.AMSASurveySiteType.ToList(), "id", "Name");
            AMSANotBillableReason = new SelectList(dbr.AMSANotBillableReason.ToList(), "id", "Name");
            ListOfOrganization = dbr.AMSAOrganization.ToList();
            AMSAOrganization = new SelectList(ListOfOrganization, "id", "Name");
            //AMSASite = new SelectList(dbr.AMSASite.ToList(), "id", "Name"); //We get this value when an organization is selected
            ListOfAmsaSite = getSiteRelatedToFirstOrganization(dbr);
            AMSASite = new SelectList(getSiteRelatedToFirstOrganization(dbr),"id","Name");
            
            AMSAProgram = new SelectList(getProgramRelatedToFirstOrganizationandFirtDepartment(dbr), "id", "Name");

            //Order by set
            this.AMSAEvent = new AMSAEvent();
            AMSAEvent.OrderBy = "perform";
            addValuesToYesNo();
            this.YesNo = new SelectList(this.ValuesYesNo, "value", "name");
            dbr.Dispose();
        }

        internal void selectMenusHaveValues(ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Search for the selected organization and see if it exists
            AMSAOrganization o = dbr.AMSAOrganization.Find(idSelectedOrganization);
            if (o == null)
                m.AddModelError("selectedOrganization", "Error! Please select an Organization");
            else
                this.AMSAEvent.AMSAOrganization = o;

            AMSASite s = dbr.AMSASite.Find(idSelectedSite);
            if (s == null)
                m.AddModelError("selectedDepartment", "Error! Please select a Department");
            else
                this.AMSAEvent.AMSASite = s;

            AMSAProgram p = dbr.AMSAProgram.Find(idSelectedProgram);
            if (p == null)
                m.AddModelError("selectedSpeciality", "Error! Please select a Speciality");
            else
                this.AMSAEvent.AMSAProgram = p;

            AMSAEventType et = dbr.AMSAEventType.Find(idSelectedEventType);
            if (et == null)
                m.AddModelError("eventType", "Please select an event type");
            else
                this.AMSAEvent.AMSAEventType = et;

            AMSASurveyType st = dbr.AMSASurveySiteType.Find(idSelectedSurveyType);
            if (st == null)
                m.AddModelError("surveyType", "Please select a survey type");
            else
                this.AMSAEvent.AMSASurveyType = st;

            if (!this.AMSAEvent.Billable) { 
            AMSANotBillableReason nbr = dbr.AMSANotBillableReason.Find(idSelectedNotBillableReason);
            if (nbr == null)
                m.AddModelError("notBillableReson", "Please select a not billable reason");
            else
                this.AMSAEvent.AMSANotBillableReason = nbr;
            }

            
            dbr.Dispose();
        }

        //Gets organizations to show on first load for event creation
        private List<AMSASite> getSiteRelatedToFirstOrganization(AMSAReportContext dbr)
        {
            List<AMSASite> lst = new List<AMSASite>();
            if(this.ListOfOrganization.Count() > 0) {
                int idOrganization = this.ListOfOrganization[0].id;
                lst = dbr.AMSASite.Where(m => m.AMSAOrganization.id == idOrganization).ToList();
            }
            return lst;
        }

        //Gets programs to be shown on first load for event creation
        private List<AMSAProgram> getProgramRelatedToFirstOrganizationandFirtDepartment(AMSAReportContext dbr)
        {
            List<AMSAProgram> lst = new List<AMSAProgram>();
            List<AMSAProgramSite> lstProgramSite = new List<AMSAProgramSite>();
            if (this.ListOfAmsaSite.Count() > 0)
            {
                int idAMSASite = this.ListOfAmsaSite[0].id;
                lstProgramSite = dbr.AMSAProgramSite.Where(m => m.AMSASite.id == idAMSASite).ToList();
            }
            foreach(var i in lstProgramSite)
            {
                AMSAProgram auxProgram = dbr.AMSAProgram.Where(m => m.id == i.AMSAProgram.id).FirstOrDefault();
                if(!lst.Contains(auxProgram))
                {
                    lst.Add(auxProgram);
                }
            }
            
            return lst;
        }

        public void addValuesToYesNo()
        {
            this.ValuesYesNo = new List<YesNo>();
            this.AMSAEvent.Billable = true;
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

        public YesNo() { }
    }
}