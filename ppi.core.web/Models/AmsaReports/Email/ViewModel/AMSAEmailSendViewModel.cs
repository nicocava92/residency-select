using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.Email.ViewModel
{
    public class AMSAEmailSendViewModel
    {
        public AMSAEmail EmailInvitation { get; set; }
        public AMSAEmail EmailReminder { get; set; }
        public SelectList Events { get; set; }
        public List<AMSAParticipant> lstParticipantsForInvitation { get; set; }
        public List<AMSAParticipant> lstParticipantForReminder { get; set; }
        public int idSelectedEvent { get; set; }

        public AMSAEmailSendViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            List<AMSAEvent> lstE = dbr.AMSAEvent.ToList();
            lstE.Insert(0, new AMSAEvent{ id = 0, Name = "Select an Event" });
            Events = new SelectList(lstE, "id", "Name");
            dbr.Dispose();
        }

        public void loadParticipants()
        {
            getListParticipantsForInvitation();
            getListParticipantsForReminder();
        }

        //Call before get list for Reminder
        public void getListParticipantsForInvitation() {
            if(idSelectedEvent > 0) { 
                AMSAReportContext dbr = new AMSAReportContext();
                lstParticipantsForInvitation = dbr.AMSAParticipant.Where(m => m.Invitation_date == null && m.AMSAEvent.id == idSelectedEvent).ToList();
                dbr.Dispose();
            }
        }
        //Call after get list for Invitation
        public void getListParticipantsForReminder() {
            if (idSelectedEvent > 0)
            {
                AMSAReportContext dbr = new AMSAReportContext();
                //Get users that have already received an invitation (if they received an invitation this means that they are ready for a reminder
                //Always keep users on the reminder list
                lstParticipantForReminder = dbr.AMSAParticipant.Where(m => m.Invitation_date != null && m.AMSAEvent.id == idSelectedEvent).ToList();
                dbr.Dispose();
            }
        }


    }
}