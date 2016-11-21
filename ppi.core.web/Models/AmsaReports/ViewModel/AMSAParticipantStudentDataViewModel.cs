using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PPI.Core.Web.Models.AmsaReports;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    //Student data class used to create new student data and upload to the database
    public class AMSAParticipantStudentDataViewModel
    {
        public AmsaReportStudentData Data { get; set; }
        public int idSelectedEvent { get; set; }
        public SelectList Events{ get; set; }
        public List<AMSAEvent> LstEvents { get; set; }
        
        public AMSAParticipantStudentDataViewModel()
        {
            //Load data to show on view and to make selections
            AMSAReportContext dbr = new AMSAReportContext();
            this.addEvents(dbr);

            dbr.Dispose();
        }

        public void addEvents(AMSAReportContext dbr)
        {
            this.LstEvents = dbr.AMSAEvent.ToList();
            //this.LstEvents.Insert(0,new AMSAEvent { id = 0, Name = "Show All"});
            this.Events = new SelectList(this.LstEvents, "id", "Name");
        }

        //Save new student data
        public void SaveNew()
        {
            //Get selected event
            AMSAReportContext db = new AMSAReportContext();
            AMSAEvent e = db.AMSAEvent.Find(this.idSelectedEvent);
            AmsaReportStudentData s = new AmsaReportStudentData();
            s.CompletionDate = this.Data.CompletionDate;
            s.RegistrationDate = this.Data.RegistrationDate;
            s.PersonId = this.Data.PersonId;
            s.FirstName = this.Data.FirstName;
            s.LastName = this.Data.LastName;
            s.Stanine_Drive = this.Data.Stanine_Drive;
            s.Stanine_Structure = this.Data.Stanine_Structure;
            s.Stanine_Conceptual = this.Data.Stanine_Conceptual;
            s.Stanine_Flexibility = this.Data.Stanine_Flexibility;
            s.Stanine_Mastery = this.Data.Stanine_Mastery;
            s.Stanine_Ambition = this.Data.Stanine_Ambition;
            s.Stanine_Power = this.Data.Stanine_Power;
            s.Stanine_Assertiveness = this.Data.Stanine_Assertiveness;
            s.Stanine_Liveliness = this.Data.Stanine_Liveliness;
            s.Stanine_Composure = this.Data.Stanine_Composure;
            s.Stanine_Positivity = this.Data.Stanine_Positivity;
            s.Stanine_Awareness = this.Data.Stanine_Awareness;
            s.Stanine_Cooperativeness = this.Data.Stanine_Cooperativeness;
            s.Stanine_Sensitivity = this.Data.Stanine_Sensitivity;
            s.Stanine_Humility = this.Data.Stanine_Humility;
            s.AMSAEvent = e;
            db.lstStudentsForReport.Add(s);
            db.SaveChanges();
            db.Dispose();
        }

        public void updateStudentData()
        {
            AMSAReportContext db = new AMSAReportContext();
            AMSAEvent e = db.AMSAEvent.Find(this.idSelectedEvent);
            AmsaReportStudentData s = db.lstStudentsForReport.Find(this.Data.Id);
            s.CompletionDate = this.Data.CompletionDate;
            s.RegistrationDate = this.Data.RegistrationDate;
            s.PersonId = this.Data.PersonId;
            s.FirstName = this.Data.FirstName;
            s.LastName = this.Data.LastName;
            s.Stanine_Drive = this.Data.Stanine_Drive;
            s.Stanine_Structure = this.Data.Stanine_Structure;
            s.Stanine_Conceptual = this.Data.Stanine_Conceptual;
            s.Stanine_Flexibility = this.Data.Stanine_Flexibility;
            s.Stanine_Mastery = this.Data.Stanine_Mastery;
            s.Stanine_Ambition = this.Data.Stanine_Ambition;
            s.Stanine_Power = this.Data.Stanine_Power;
            s.Stanine_Assertiveness = this.Data.Stanine_Assertiveness;
            s.Stanine_Liveliness = this.Data.Stanine_Liveliness;
            s.Stanine_Composure = this.Data.Stanine_Composure;
            s.Stanine_Positivity = this.Data.Stanine_Positivity;
            s.Stanine_Awareness = this.Data.Stanine_Awareness;
            s.Stanine_Cooperativeness = this.Data.Stanine_Cooperativeness;
            s.Stanine_Sensitivity = this.Data.Stanine_Sensitivity;
            s.Stanine_Humility = this.Data.Stanine_Humility;
            s.AMSAEvent = e;
            db.SaveChanges();
            db.Dispose();
        }

        public void loadStudentData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AmsaReportStudentData s = dbr.lstStudentsForReport.Find(id);
            AMSAParticipantStudentDataViewModel svm = new AMSAParticipantStudentDataViewModel();
            svm.Data = s;
            svm.idSelectedEvent = s.AMSAEvent.id;
            dbr.Dispose();
        }

    }
}