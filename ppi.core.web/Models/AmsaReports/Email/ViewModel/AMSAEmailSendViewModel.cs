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
                lstParticipantForReminder = dbr.AMSAParticipant.Where(m => m.Reminder_date == null && m.AMSAEvent.id == idSelectedEvent).ToList();
                //Eliminate the participants that are in the invitation list (this means they have a reminder date as null
                //Because the invitation has not yet been set
                List<AMSAParticipant> auxLstParticipantToDelete = new List<AMSAParticipant>();
                //Get list of participants to delete
                foreach (AMSAParticipant p in lstParticipantForReminder)
                {
                    //Contains not working need to perform the loop through manuall
                    foreach (AMSAParticipant pWaitingInvitation in lstParticipantsForInvitation)
                    {
                        if (p.Id == pWaitingInvitation.Id)
                            auxLstParticipantToDelete.Add(p);
                    }
                }
                //Remove participants that are on the invitation list from the reminder list
                foreach (AMSAParticipant pa in auxLstParticipantToDelete)
                {
                    lstParticipantForReminder.Remove(pa);
                }
                dbr.Dispose();
            }
        }


    }
}