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
        public void Perform(HttpRequestBase request, ModelStateDictionary m)
        {
            List<AMSAParticipant> lstParticipants = new List<AMSAParticipant>();
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
                        
                        AMSAParticipant p = new AMSAParticipant();
                        p.FirstName = values[0];
                        p.LastName = values[1];
                        p.PrimaryEmail = values[2];
                        p.AMSACode = values[3];
                        p.AAMCNumber = values[4];
                        p.Gender = values[5];
                        p.Title = values[6];
                        lstParticipants.Add(p);
                    }
                    c++;
                }
                //After first line - which is the description file - add participant
                i++;
            }

            //After all of the Participants that we are tryign to insert into the database are added into the list, then we try to add them or return errors if there are problems

            bool finished = false; //Checks if we are done with all users, used to return errors in case that we have to go through all of them with errors in some of the insertions.
            int position = 1;
            int ammountOfParticipants = lstParticipants.Count;
            foreach (AMSAParticipant p in lstParticipants)
            {


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
                    this.checkAlreadyAssignedToEvent(p, m);
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
            if (p.PrimaryEmail == null)
            {
                this.Errors += " Primary e - mail missing for " + p.FirstName;
                m.AddModelError("Participant", "Primary e - mail missing for " + p.FirstName);
            }

            if (p.PrimaryEmail != null)
            {
                if (p.FirstName == null)
                {
                    this.Errors += " Name missing for " + p.PrimaryEmail;
                    m.AddModelError("Participant", " Name missing for " + p.PrimaryEmail);
                }
                else
                {
                    //Check size of name
                    if (p.FirstName.Length >= 3)
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
                    if (p.LastName.Length >= 3)
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
