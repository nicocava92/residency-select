using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Excel;

using PPI.Core.Web.Models.AmsaReports.Event;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSAReportStudentDataUploadViewModel
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

        public AMSAReportStudentDataUploadViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            LstEvents = dbr.AMSAEvent.ToList();
            LstEvents.Insert(0, new AMSAEvent { id = 0, Name = "-- Please select an Event --" });
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

        //Returns model errors if users could not be inserted.
        /*
        Check if data was inserted correctly for the different
            */
        public void PerformStudentDataInertions(HttpRequestBase request, ModelStateDictionary m)
        {
            List<AmsaReportStudentData> lstParticipantResults = new List<AmsaReportStudentData>();
            int i = 0;
            while (i < request.Files.Count)
            {
                try { 
                    //Read files and get array
                    List<string[]> values = ReportUtilities.getDataFromCsvXlsxORXLReportData(request.Files[i]);
                    //WHere participants to be inserted into the database will be added

                    //Get list of participants from va
                    lstParticipantResults = ReportUtilities.arrayToParticipantResultsList(values);
                    //After all of the Participants that we are tryign to insert into the database are added into the list, then we try to add them or return errors if there are problems
                    i++;
                }
                catch
                {
                    m.AddModelError("Participant", "Data load failed for file # " + i);
                    i++;
                }
            }

            if (m.IsValid) { 

                int position = 1;
                int ammountOfResults = lstParticipantResults.Count;
                foreach (AmsaReportStudentData p in lstParticipantResults)
                {
                    AMSAParticipantStudentDataViewModel pvm = new AMSAParticipantStudentDataViewModel();
                    pvm.Data = p;
                    pvm.idSelectedEvent = this.idSelectedEvent;

                    this.checkModelState(p, m);

                    //We where able to store the participant
                    try
                    {
                        //Check if the participant already exists in the database
                        this.checkAlreadyAssignedToEvent(p, m);
                        if (this.Errors == "" || this.Errors == null)
                        {
                            //If participant has all of its information present then try adding him in
                            pvm.SaveNew();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        //If we go into catch if because there where not enough amsa codes to store the 
                        m.AddModelError("Participant", "From Participant with id " + p.PersonId + "in line #" + position + " File #" + i + " forward no data was inserted, problem storing to the datbase (please check data feed file and try again)" + System.Environment.NewLine);
                        this.Errors += "From Participant with id " + p.PersonId + " in line #" + position + " File #" + i + " forward no data was inserted, problem storing to the datbase (please check data feed file and try again)";
                        //If all fails then we need to return with an error to the view
                        return;
                    }

                    position++;
                    this.Errors = ""; //Clear out error check for next participant
                }
            }
        }
        //Check if the model state is valid or not
        public void checkModelState(AmsaReportStudentData p, ModelStateDictionary m)
        {
            
            if (p.PersonId == null)
            {
                this.Errors += " Id missing for " + p.FirstName;
                m.AddModelError("Participant", "Id missing for" + p.FirstName);
            }

            if (p.Status == null)
            {
                this.Errors += " Status missing for " + p.FirstName;
                m.AddModelError("Participant", "Status missing for " + p.FirstName);
            }

            if (p.PersonId != null)
            {
                if (p.FirstName == null)
                {
                    this.Errors += " Name missing for " + p.PersonId;
                    m.AddModelError("Participant", "Name missing for " + p.PersonId);
                }
                else
                {
                    //Check size of name
                    if (p.FirstName.Length < 3)
                    {
                        this.Errors += " First name needs to have 3 letters or more" + p.PersonId;
                        m.AddModelError("Participant", "First name needs to have 3 letters or more" + p.PersonId);
                    }
                }

                if (p.LastName == null)
                {
                    this.Errors += " Last name missing for " + p.PersonId;
                    m.AddModelError("Participant", "Last name missing for " + p.PersonId);
                }
                else
                {
                    //Check length of last name
                    if (p.LastName.Length < 3)
                    {
                        this.Errors += " Last name needs to have 3 letters or more" + p.PersonId;
                        m.AddModelError("Participant", "last name needs to have 3 letters or more" + p.PersonId);
                    }
                }
                if(p.RegistrationDate == null) {
                    this.Errors += " Registration date missing for" + p.PersonId;
                    m.AddModelError("Participant", "registration date missing for" + p.PersonId);
                }
                //Check for certain nulls only if the participant has a status of completed
                if (p.Status.ToUpper().Equals("COMPLETED"))
                { 
                    if (p.CompletionDate == null)
                    {
                        this.Errors += " Completion date missing for" + p.PersonId;
                        m.AddModelError("Participant", "Completion date missing for" + p.PersonId);
                    }
                    if (p.Stanine_Ambition > 9 || p.Stanine_Ambition < 1)
                    {
                        this.Errors += " Stanine Ambition needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Ambition needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Assertiveness> 9 || p.Stanine_Assertiveness  < 1)
                    {
                        this.Errors += " Stanine Assertiveness needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Assertiveness needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Awareness > 9 || p.Stanine_Awareness < 1)
                    {
                        this.Errors += " Stanine Awareness needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Awareness needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Composure > 9 || p.Stanine_Composure < 1)
                    {
                        this.Errors += " Stanine Composure needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Composure needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Conceptual > 9 || p.Stanine_Conceptual < 1)
                    {
                        this.Errors += " Stanine Conceptual needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Conceptual needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Cooperativeness > 9 || p.Stanine_Cooperativeness < 1)
                    {
                        this.Errors += " Stanine Cooperativeness needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Cooperativeness needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Drive > 9 || p.Stanine_Drive < 1)
                    {
                        this.Errors += " Stanine Drive needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Drive needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }

                    if (p.Stanine_Flexibility > 9 || p.Stanine_Flexibility < 1)
                    {
                        this.Errors += " Stanine Flexibility needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Flexibility needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Humility > 9 || p.Stanine_Humility < 1)
                    {
                        this.Errors += " Stanine Humility needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Humility needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Liveliness > 9 || p.Stanine_Liveliness < 1)
                    {
                        this.Errors += " Stanine Liveliness needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Liveliness needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Mastery > 9 || p.Stanine_Mastery < 1)
                    {
                        this.Errors += " Stanine Mastery needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Mastery needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Positivity > 9 || p.Stanine_Positivity < 1)
                    {
                        this.Errors += " Stanine Positivity needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Positivity needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Power > 9 || p.Stanine_Power < 1)
                    {
                        this.Errors += " Stanine Power needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Power needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Sensitivity > 9 || p.Stanine_Sensitivity < 1)
                    {
                        this.Errors += " Stanine Sensitivity needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Sensitivity needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                    if (p.Stanine_Structure > 9 || p.Stanine_Structure < 1)
                    {
                        this.Errors += " Stanine Structure needs to be greater than 1 and smaller than 9" + p.PersonId;
                        m.AddModelError("Participant", "Stanine Structure needs to be greater than 1 and smaller than 9" + p.PersonId);
                    }
                }
                else
                {
                    //If participant has not finished the interview then set value of 0 and don't show in listings
                    p.CompletionDate = DateTime.Now;
                    p.Stanine_Ambition = 1;
                    p.Stanine_Assertiveness = 1;
                    p.Stanine_Awareness = 1;
                    p.Stanine_Composure = 1;
                    p.Stanine_Conceptual = 1;
                    p.Stanine_Cooperativeness = 1;
                    p.Stanine_Drive = 1;
                    p.Stanine_Flexibility = 1;
                    p.Stanine_Humility = 1;
                    p.Stanine_Liveliness = 1;
                    p.Stanine_Mastery = 1;
                    p.Stanine_Positivity = 1;
                    p.Stanine_Power = 1;
                    p.Stanine_Sensitivity = 1;
                    p.Stanine_Structure = 1;
                }
            }

        }

        /*
            Checks if the participant is already assigned to the event and already has data for the event
            If the participant already exsists in both listings then update data
            If the participant doesnt exist then add only to Report list data
        */

        public void checkAlreadyAssignedToEvent(AmsaReportStudentData pa, ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AmsaReportStudentData p = dbr.lstStudentsForReport.Where(r => r.PersonId.ToUpper().Equals(pa.PersonId.ToUpper()) && r.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if (p != null)
            {
                //If the user already exists in the event then save the user and return message that the user was updated (will be seen as an error)
                p.RegistrationDate = pa.RegistrationDate;
                p.CompletionDate = pa.CompletionDate;
                //Depending on the status we are going to either save the result or update the participant status
                p.Status = pa.Status;
                p.Stanine_Ambition = pa.Stanine_Ambition;
                p.Stanine_Assertiveness = pa.Stanine_Assertiveness;
                p.Stanine_Awareness = pa.Stanine_Awareness;
                p.Stanine_Composure = pa.Stanine_Composure;
                p.Stanine_Conceptual = pa.Stanine_Conceptual;
                p.Stanine_Cooperativeness = pa.Stanine_Cooperativeness;
                p.Stanine_Drive = pa.Stanine_Drive;
                p.Stanine_Flexibility = pa.Stanine_Flexibility;
                p.Stanine_Humility = pa.Stanine_Humility;
                p.Stanine_Liveliness = pa.Stanine_Liveliness;
                p.Stanine_Mastery = pa.Stanine_Mastery;
                p.Stanine_Positivity = pa.Stanine_Positivity;
                p.Stanine_Power = pa.Stanine_Power;
                p.Stanine_Sensitivity = pa.Stanine_Sensitivity;
                p.Stanine_Structure = pa.Stanine_Structure;
                dbr.SaveChanges();
                //Add errors so we dont store it again, even tho no errors are present.
                //Users will see error message on the view informing them that the user has been updated
                this.Errors += "Information has been updated for participant " + p.FirstName + " " + p.LastName + " " + p.PersonId;
                m.AddModelError("Participant","Information has been updated for participant " + p.FirstName + " " + p.LastName + " " + p.PersonId);
            }

            //After updating the results then update the participant
            AMSAParticipant aParticipant = dbr.AMSAParticipant.Where(o => o.AMSACode.ToUpper().Equals(pa.PersonId) && o.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if (aParticipant != null)
            {
                //If participant exists then update his status | if his status goes from something different
                aParticipant.Status = pa.Status;
                dbr.SaveChanges();
            }
            
            
            dbr.Dispose();
        }
        //Save update event date when changes are performed to users
        internal void updateEventUpdate()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent e = dbr.AMSAEvent.Find(this.idSelectedEvent);
            if(e!=null)
            {
                e.Updated = DateTime.Now;
                dbr.SaveChanges();
            }
            dbr.Dispose();
        }
    }
}
