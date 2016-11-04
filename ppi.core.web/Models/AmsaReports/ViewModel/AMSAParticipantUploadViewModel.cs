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
        [DisplayName("Upload CSV For Event")]
        public HttpPostedFile csvFile { get; set; }
        public AMSAEvent selectedEvent { get; set; }
        //Store errors on upload so further users are added even if other users generated errors, inform multiple errors at once without stopping the application form finishing its normal course
        public string Errors { get; set; }

        public List<AMSAEvent> LstEvents { get; set; }

        public AMSAParticipantUploadViewModel()
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
                m.AddModelError("UploadError", "Please upload a .CSV file");
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
            int eventId = LstEvents[0].id;
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
            int i = 0;
            while (i < request.Files.Count)
            {
                HttpPostedFileBase UploadedFile = request.Files[0];
                
                // Use the InputStream to get the actual stream sent.
                StreamReader csvreader = new StreamReader(UploadedFile.InputStream);
                var c = 0;
                while (!csvreader.EndOfStream)
                {
                    var line = csvreader.ReadLine();
                    var values = line.Split(',');
                    //If not on first element 
                    if (c > 0) { 
                        //Try to save if there are errors let user know the participants that where not uploaded.. store the participants e-mail address and line number to let the user know
                        AMSAParticipantViewModel pvm = new AMSAParticipantViewModel();
                        AMSAParticipant p = new AMSAParticipant();
                        p.FirstName = values[0];
                        p.LastName = values[1];
                        p.PrimaryEmail = values[2];
                        p.AMSACode = values[3];
                        p.AAMCNumber = values[4];
                        p.Gender = values[5];
                        p.Title = values[6];
                        pvm.AMSAParticipant = p;
                        pvm.idSelectedEvent = this.idSelectedEvent;
                        this.checkModelState(p, m);
                        if (m.IsValid)
                        {
                            //We where able to store the participant
                            try {
                                //Check if the participant already exists in the database
                                this.checkAlreadyAssignedToEvent(p);
                                if (this.Errors != "") {
                                    pvm.saveNewParticipant();
                                }
                                else
                                    m.AddModelError("Participant", this.Errors);
                            }
                            catch
                            {
                                //If we go into catch if because there where not enough amsa codes to store the 
                                m.AddModelError("Participant", "From participant with e-mail address " + p.PrimaryEmail + "in line " + c + "no participants were inserted, not enough codes available");
                            }
                        }

                    }
                    c++;
                }
                //After first line - which is the description file - add participant
                i++;
            }
        }
        //Check if the model state is valid or not
        public void checkModelState(AMSAParticipant p, ModelStateDictionary m)
        {
            if (p.PrimaryEmail == null)
                this.Errors += " Primary e-mail missing for " + p.FirstName;
            if (p.PrimaryEmail != null) { 
                if (p.FirstName == null)
                    this.Errors+= " Name missing for " + p.PrimaryEmail;
                if (p.LastName == null)
                    this.Errors += " Last name missing for " + p.PrimaryEmail;
                if (p.Gender == null)
                    this.Errors += " Gender missing for " + p.PrimaryEmail;
                if (p.Title == null)
                    this.Errors += " Title missing for " + p.PrimaryEmail;
            }

        }

        public void checkAlreadyAssignedToEvent(AMSAParticipant pa)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAParticipant p = dbr.AMSAParticipant.Where(r => r.PrimaryEmail.Equals(pa.PrimaryEmail) && r.AMSAEvent.id == this.idSelectedEvent).FirstOrDefault();
            if (p != null)
            {
                this.Errors += "Participant with e-mail address " + pa.PrimaryEmail + "already assigned to the selected event";
            }
            dbr.Dispose();
        }
    }
}