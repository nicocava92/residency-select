using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSAReportStudentDataUploadViewModel
    {
        [DisplayName("Event")]
        public SelectList Events { get; set; }
        public int idSelectedEvent { get; set; }
        [DisplayName("Upload CSV For Event")]
        public HttpPostedFile csvFile { get; set; }
        public AMSAEvent selectedEvent { get; set; }
        //Store errors on upload so further users are added even if other users generated errors, inform multiple errors at once without stopping the application form finishing its normal course
        public string Errors { get; set; }

        public List<AMSAEvent> LstEvents { get; set; }

        public AMSAReportStudentDataUploadViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            LstEvents = dbr.AMSAEvent.ToList();
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
                HttpPostedFileBase UploadedFile = request.Files[0];
                //WHere participants to be inserted into the database will be added

                // Use the InputStream to get the actual stream sent.
                StreamReader csvreader = new StreamReader(UploadedFile.InputStream);
                var c = 0;
                while (!csvreader.EndOfStream)
                {
                    var line = csvreader.ReadLine();
                    var values = line.Split(',');
                    //If not on first element 
                    if (c > 0)
                    {
                        //Try to save if there are errors let the user know
                        
                        AmsaReportStudentData p = new AmsaReportStudentData();
                        p.FirstName = values[0];
                        p.LastName = values[1];
                        p.PersonId = values[2];
                        p.RegistrationDate = Convert.ToDateTime(values[3]);
                        p.CompletionDate = Convert.ToDateTime(values[4]);
                        p.Stanine_Ambition = Convert.ToInt32(values[5]);
                        p.Stanine_Assertiveness = Convert.ToInt32(values[7]);
                        p.Stanine_Awareness = Convert.ToInt32(values[6]);
                        p.Stanine_Composure = Convert.ToInt32(values[7]);
                        p.Stanine_Conceptual = Convert.ToInt32(values[8]);
                        p.Stanine_Cooperativeness = Convert.ToInt32(values[9]);
                        p.Stanine_Drive = Convert.ToInt32(values[10]);
                        p.Stanine_Flexibility = Convert.ToInt32(values[11]);
                        p.Stanine_Humility = Convert.ToInt32(values[12]);
                        p.Stanine_Liveliness = Convert.ToInt32(values[13]);
                        p.Stanine_Mastery = Convert.ToInt32(values[14]);
                        p.Stanine_Positivity = Convert.ToInt32(values[15]);
                        p.Stanine_Power = Convert.ToInt32(values[16]);
                        p.Stanine_Sensitivity = Convert.ToInt32(values[17]);
                        p.Stanine_Structure = Convert.ToInt32(values[18]);
                        lstParticipantResults.Add(p);
                    }
                    c++;
                }
                //After first line - which is the description file - add participant
                i++;
            }

            //After all of the Participants that we are tryign to insert into the database are added into the list, then we try to add them or return errors if there are problems

            bool finished = false; //Checks if we are done with all users, used to return errors in case that we have to go through all of them with errors in some of the insertions.
            int position = 1;
            int ammountOfResults = lstParticipantResults.Count;
            foreach (AmsaReportStudentData p in lstParticipantResults)
            {


                if (position == ammountOfResults)
                    finished = true;

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
                catch
                {
                    //If we go into catch if because there where not enough amsa codes to store the 
                    m.AddModelError("Participant", "From Participant with id " + p.PersonId + "in line #" + position + " File #" + i + " forward no data was inserted, problem storing to the datbase (please check data feed file and try again)" + System.Environment.NewLine);
                    this.Errors += "From Participant with id " + p.PersonId + "in line #" + position + " File #" + i + " forward no data was inserted, problem storing to the datbase (please check data feed file and try again)";
                    //If all fails then we need to return with an error to the view
                    return;
                }

                position++;
                this.Errors = ""; //Clear out error check for next participant
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

            if (p.PersonId != null)
            {
                if (p.FirstName == null)
                {
                    this.Errors += " Name missing for " + p.PersonId;
                    m.AddModelError("Participant", " Name missing for " + p.PersonId);
                }
                else
                {
                    //Check size of name
                    if (p.FirstName.Length >= 3)
                    {
                        this.Errors += " First name needs to have 3 letters or more" + p.PersonId;
                        m.AddModelError("Participant", "First name needs to have 3 letters or more" + p.PersonId);
                    }
                }

                if (p.LastName == null)
                {
                    this.Errors += " Last name missing for " + p.PersonId;
                    m.AddModelError("Participant", " Last name missing for " + p.PersonId);
                }
                else
                {
                    //Check length of last name
                    if (p.LastName.Length >= 3)
                    {
                        this.Errors += " Last name needs to have 3 letters or more" + p.PersonId;
                        m.AddModelError("Participant", "last name needs to have 3 letters or more" + p.PersonId);
                    }
                }
                if(p.RegistrationDate == null) {
                    this.Errors += " Registration date missing for" + p.PersonId;
                    m.AddModelError("Participant", "registration date missing for" + p.PersonId);
                }
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

        }

        public void checkAlreadyAssignedToEvent(AmsaReportStudentData pa, ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AmsaReportStudentData p = dbr.lstStudentsForReport.Where(r => r.PersonId.ToUpper().Equals(pa.PersonId.ToUpper()) && r.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if (p != null)
            {
                this.Errors += " Participant with id" + pa.PersonId + " already assigned to the selected event";
                m.AddModelError("Participant", "Participant with id " + pa.PersonId+ " already assigned to the selected event");
            }
            dbr.Dispose();
        }
    }
}
