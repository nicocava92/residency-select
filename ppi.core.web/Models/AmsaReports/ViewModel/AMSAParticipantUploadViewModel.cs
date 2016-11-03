using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Linq;
using System.Text;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAParticipantUploadViewModel
    {
        public SelectList Events { get; set; }
        public int idSelectedEvent { get; set; }
        public HttpPostedFile csvFile { get; set; }
        public AMSAEvent selectedEvent { get; set; }

        public List<AMSAEvent> lstEvents { get; set; }

        public AMSAParticipantUploadViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            lstEvents = dbr.AMSAEvent.ToList();
            Events = new SelectList(lstEvents, "id", "Name");
        }

        //Check the inserted information (Event selected and file uploaded)
        public void checkData(ModelStateDictionary m) {
            //Check file type to make sure that its .csv
            if (!this.csvFile.GetType().Equals(".csv"))
            {
                //If fyle type is not .csv let the user now that they need to upload a .csv to the form for it to work correctly
                m.AddModelError("UploadError", "Please upload a .CSV file");
            }
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent e = dbr.AMSAEvent.Where(r => r.id == this.idSelectedEvent).FirstOrDefault();
            if(e == null)
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
    }
}