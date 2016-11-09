using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class ParticipantListViewModel
    {
        //List of participants to show on view
        public List<AMSAParticipant> LstParticipants { get; set; }

        //List of possible Events
        public SelectList Events { get; set; }
        //Get all events, used to add to the top of te list an "event" that can be selected to show all events
        public List<AMSAEvent> LstEvents { get; set; }

        public int idSelectedEvent { get; set; }

        public string Search { get; set; }

        public ParticipantListViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            LstParticipants = dbr.AMSAParticipant.ToList();
            loadAllEvents(dbr);
            
            dbr.Dispose();
        }

        public List<AMSAParticipant> getGetLstParticipants()
        {
            return this.LstParticipants;
        }

        public void loadAllEvents(AMSAReportContext dbr)
        {
            LstEvents = dbr.AMSAEvent.ToList();
            AMSAEvent e = new AMSAEvent();
            e.Name = "Show All";
            e.id = 0;
            LstEvents.Insert(0, e);
            Events = new SelectList(LstEvents, "id", "Name");
        }
    }
}