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

        public int idSelectedEvent { get; set; }

        public ParticipantListViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            LstParticipants = dbr.AMSAParticipant.ToList();
            Events = new SelectList(dbr.AMSAEvent.ToList(), "id", "Name");
            dbr.Dispose();
        }

    }
}