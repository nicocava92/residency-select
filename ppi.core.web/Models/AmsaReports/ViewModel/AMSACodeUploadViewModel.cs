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
    public class AMSACodeUploadViewModel
    {
        [DisplayName("Event")]
        public SelectList Events { get; set; }
        public int idSelectedEvent { get; set; }
        [DisplayName("Upload CSV With AMSA Codes For Event")]
        public HttpPostedFile csvFile { get; set; }
        public AMSAEvent selectedEvent { get; set; }
        //Store errors on upload so further users are added even if other users generated errors, inform multiple errors at once without stopping the application form finishing its normal course
        public string Errors { get; set; }

        public List<AMSAEvent> LstEvents { get; set; }

        public AMSACodeUploadViewModel()
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
        public void PerformCodeInsertions(HttpRequestBase request, ModelStateDictionary m)
        {
            List<AMSACode> lstCodes = new List<AMSACode>();
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
                        //Try to save if there are errors let user know the participants that where not uploaded.. store the participants e-mail address and line number to let the user know

                        //AMSAParticipant p = new AMSAParticipant();
                        //p.FirstName = values[0];
                        //p.LastName = values[1];
                        //p.PrimaryEmail = values[2];
                        //p.AMSACode = values[3];
                        //p.AAMCNumber = values[4];
                        //p.Gender = values[5];
                        //p.Title = values[6];
                        //lstParticipants.Add(p);
                        AMSACode auxCode = new AMSACode();
                        auxCode.Code = values[0];
                        lstCodes.Add(auxCode);
                    }
                    c++;
                }
                //After first line - which is the description file - add participant
                i++;
            }

            //After all of the Participants that we are tryign to insert into the database are added into the list, then we try to add them or return errors if there are problems

            bool finished = false; //Checks if we are done with all users, used to return errors in case that we have to go through all of them with errors in some of the insertions.
            int position = 1;
            int ammountOfParticipants = lstCodes.Count;
            foreach (AMSACode itemCode in lstCodes)
            {


                if (position == ammountOfParticipants)
                    finished = true;

                AMSACodeViewModel cvm = new AMSACodeViewModel();
                cvm.AMSACode = itemCode;
                cvm.idSelectedEvent = this.idSelectedEvent;

                try
                {
                    //Check if the participant already exists in the database
                    this.checkAlreadyAssignedToEvent(itemCode, m);
                    if (this.Errors == "" || this.Errors == null)
                    {
                        //If participant has all of its information present then try adding him in
                        cvm.saveNewCode();
                    }
                }
                catch
                {
                    //If we go into catch if because there where not enough amsa codes to store the 
                    m.AddModelError("Code", "Process stopped when trying to insert code " + itemCode.Code + "in line #" + position + " File #" + i + " forward no AMSA Codes were inserted after this row as well, please make sure that you inserted the codes correctly." + System.Environment.NewLine);
                    this.Errors += "Process stopped when trying to insert code " + itemCode.Code + "in line #" + position + " File #" + i + " forward no AMSA Codes were inserted after this row as well, please make sure that you inserted the codes correctly.";
                }

                position++;
                this.Errors = ""; //Clear out error check for next participant
            }

        }
        
        

        public void checkAlreadyAssignedToEvent(AMSACode code, ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(r=> r.Code == code.Code).FirstOrDefault();
            if (c != null)
            {
                this.Errors += " Code: " + c.Code + " already in the system, please insert a different code";
                m.AddModelError("Code", "Code: " + c.Code + " already in the system, please insert a different code");
            }
            dbr.Dispose();
        }
    }
}