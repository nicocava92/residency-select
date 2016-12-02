using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Linq;
using System.Text;
using System.ComponentModel;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSAParticipantUploadViewModel
    {
        [DisplayName("Event")]
        public SelectList Events { get; set; }
        public int idSelectedEvent { get; set; }
        [DisplayName("Upload .csv, .xls, .xlsx For Event")]
        public HttpPostedFile csvFile { get; set; }
        public AMSAEvent selectedEvent { get; set; }
        //Store errors on upload so further users are added even if other users generated errors, inform multiple errors at once without stopping the application form finishing its normal course
        public string Errors { get; set; }

        public List<AMSAEvent> LstEvents { get; set; }

        public AMSAParticipantUploadViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            LstEvents = dbr.AMSAEvent.ToList();
            LstEvents.Insert(0, new AMSAEvent { id = 0, Name = "-- Please Select an Event --" });
            Events = new SelectList(LstEvents, "id", "Name");
        }

        //Check the inserted information (Event selected and file uploaded)
        public void checkData(ModelStateDictionary m)
        {
            //Check file type to make sure that its .csv
            if (!this.csvFile.GetType().Equals(".csv"))
            {
                //If fyle type is not .csv let the user now that they need to upload a .csv to the form for it to work correctly
                m.AddModelError("UploadError", "Please upload a .CSV file" + System.Environment.NewLine);
            }
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent e = dbr.AMSAEvent.Where(r => r.id == this.idSelectedEvent).FirstOrDefault();
            if (e == null)
            {
                m.AddModelError("AMSAEvent", "Please select a valid event");
            }
            else
            {
                this.selectedEvent = e;
            }
            dbr.Dispose();
        }

        //Returns a csv with errors if errors are present (this is not going to by asyncronic)
        public StringBuilder saveError()
        {
            StringBuilder csvErrors = null;

            //Read CSV 
            String st = File.ReadAllText(this.csvFile.ToString());
            //If this reads the file correctly then read the content that is inside the file


            csvErrors.AppendLine("Error");

            //Add a new line if there is an error

            return csvErrors;
        }


        public int AMASCodeCount()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Get the the event on first 
            int eventId = idSelectedEvent;
            List<AMSACode> lstCodes = dbr.AMSACodes.Where(m => !m.Used && m.AMSAEvent.id == eventId).ToList();
            int ammountOfCodes = lstCodes.Count();
            dbr.Dispose();
            return ammountOfCodes;
        }


        //Returns model errors if users could not be inserted.
        /*
        Check if data was inserted correctly for the different
            */
        public void PerformUserInsertionts(HttpRequestBase request, ModelStateDictionary m) {
            List<AMSAParticipant> lstParticipants = new List<AMSAParticipant>();
            int i = 0;


            while (i < request.Files.Count)
            {
                try
                {
                    //Read files and get array
                    List<string[]> values = ReportUtilities.getDataFromCsvXlsxORXLAMSAParticipants(request.Files[i]);
                    //WHere participants to be inserted into the database will be added

                    //Get list of participants from va
                    lstParticipants = ReportUtilities.arrayToAMSAParticipants(values);
                    //After all of the Participants that we are tryign to insert into the database are added into the list, then we try to add them or return errors if there are problems
                    i++;
                }
                catch
                {
                    m.AddModelError("Participant", "Data load failed for file # " + i + " please check that all information is inserted correctly");
                    i++;
                }
            }


            //After all of the Participants that we are tryign to insert into the database are added into the list, then we try to add them or return errors if there are problems

            bool finished = false; //Checks if we are done with all users, used to return errors in case that we have to go through all of them with errors in some of the insertions.
            int position = 1;
            int ammountOfParticipants = lstParticipants.Count;
            foreach(AMSAParticipant p in lstParticipants) {


                if (position == ammountOfParticipants)
                    finished = true;

                AMSAParticipantViewModel pvm = new AMSAParticipantViewModel();
                pvm.AMSAParticipant = p;
                pvm.idSelectedEvent = this.idSelectedEvent;

                this.checkModelState(p, m);
                
                    //We where able to store the participant
                    try
                    {
                        //Check if the participant already exists in the database
                        this.checkAlreadyAssignedToEvent(p,m);
                        if (this.Errors == "" || this.Errors == null)
                        {
                            //If participant has all of its information present then try adding him in
                            pvm.saveNewParticipant();
                        }
                    }
                    catch
                    {
                        //If we go into catch if because there where not enough amsa codes to store the 
                        m.AddModelError("Participant", "From Participant with e-mail address " + p.PrimaryEmail + "in line #" + position + " File #" + i + " forward no participants were inserted after this row as well, not enough codes available. Please insert more codes to upload Participants" + System.Environment.NewLine);
                        this.Errors += " From participant with e-mail address " + p.PrimaryEmail + "in line #" + position + " File #" + i + " forward no participants were inserted after this row as well, not enough codes available. Please insert more codes to upload Participants";
                        //If all fails then we need to return with an error to the view
                        return;
                    }
                
                position++;
                this.Errors = ""; //Clear out error check for next participant
            }

        }
        //Check if the model state is valid or not
        public void checkModelState(AMSAParticipant p, ModelStateDictionary m)
        {
            if (p.PrimaryEmail == null) {
                this.Errors += " Primary e-mail missing for " + p.FirstName;
                m.AddModelError("Participant", "Primary e - mail missing for " + p.FirstName);
            }
            else
            {
                //Make sure the e-mail is longer than 3 characters
                if(p.PrimaryEmail.Length <= 3)
                {
                    this.Errors += " Primary e-mail needs to be longer than 3 characters" + p.PrimaryEmail;
                    m.AddModelError("Participant", " Primary e-mail needs to be longer than 3 characters" + p.PrimaryEmail);
                }
            }
               
            if (p.PrimaryEmail != null) {
                if (p.FirstName == null)
                {
                    this.Errors += " Name missing for " + p.PrimaryEmail;
                    m.AddModelError("Participant", " Name missing for " + p.PrimaryEmail);
                }
                else
                {
                    //Check size of name
                    if(p.FirstName.Length <= 3)
                    {
                        this.Errors += " First name needs to have 3 letters or more" + p.FirstName;
                        m.AddModelError("Participant", "First name needs to have 3 letters or more" + p.FirstName);
                    }
                }
                    
                if (p.LastName == null)
                {
                    this.Errors += " Last name missing for " + p.PrimaryEmail;
                    m.AddModelError("Participant", " Last name missing for " + p.PrimaryEmail);
                }
                else
                {
                    //Check length of last name
                    if(p.LastName.Length <= 3)
                    {
                        this.Errors += " Last name needs to have 3 letters or more" + p.LastName;
                        m.AddModelError("Participant", "last name needs to have 3 letters or more" + p.LastName);
                    }
                }
                    
                if (p.Gender == null)
                {
                    this.Errors += " Gender missing for " + p.PrimaryEmail;
                    m.AddModelError("Participant", " Gender missing for " + p.PrimaryEmail);
                }
                  
                if (p.Title == null)
                {
                    this.Errors += " Title missing for " + p.PrimaryEmail;
                    m.AddModelError("Participant", " Title missing for " + p.PrimaryEmail);
                }
                  
            }

        }

        public void checkAlreadyAssignedToEvent(AMSAParticipant pa, ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAParticipant p = dbr.AMSAParticipant.Where(r => r.PrimaryEmail.Equals(pa.PrimaryEmail) && r.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if (p != null)
            {
                this.Errors += " Participant with e-mail address " + pa.PrimaryEmail + " already assigned to the selected event";
                m.AddModelError("Participant", "Participant with e-mail address " + pa.PrimaryEmail + " already assigned to the selected event");
            }
            dbr.Dispose();
        }
    }
}