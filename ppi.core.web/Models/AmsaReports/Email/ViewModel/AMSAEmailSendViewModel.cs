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

            //Set new lists for emails
            lstParticipantForReminder = new List<AMSAParticipant>();
            lstParticipantsForInvitation = new List<AMSAParticipant>();

            dbr.Dispose();
        }

        public void loadParticipants()
        {
            getListParticipantsForInvitation();
            getListParticipantsForReminder();
        }

        //Call before get list for Reminder
        public void getListParticipantsForInvitation() {
            //List to store information about participants that should be sent invitations (we will filter out later the ones that should not be sent the reminders
            
            List<AMSAParticipant> auxLstParticipants = new List<AMSAParticipant>();
            if (idSelectedEvent > 0) { 
                AMSAReportContext dbr = new AMSAReportContext();
                auxLstParticipants = dbr.AMSAParticipant.Where(m => m.Invitation_date == null && m.AMSAEvent.id == idSelectedEvent).ToList();
                if (auxLstParticipants != null) { 
                    foreach (AMSAParticipant p in auxLstParticipants)
                    {
                        //if the participant has not finished the event then we show it else we don't
                        if (!p.finishedEvent())
                        {
                            lstParticipantsForInvitation.Add(p);
                        }
                        else
                        {
                            //if the participant has finished check if the status is completed and if not change it to this
                            if (!(p.Status.ToUpper().Trim().Equals("COMPLETED") || p.Status.ToUpper().Trim().Equals("COMPLETE"))){
                                p.Status = "COMPLETED";
                                p.saveChanges();
                            }
                        }
                    }
                    //Order the list alphabetically
                    lstParticipantsForInvitation.OrderBy(m => m.FirstName);
                }
            }
        }
        //Call after get list for Invitation
        public void getListParticipantsForReminder() {
            //List to store information about participants that should be sent reminders (we will filter out later the ones that should not be sent the reminders
            List<AMSAParticipant> auxLstParticipants = new List<AMSAParticipant>();
            if (idSelectedEvent > 0)
            {
                AMSAReportContext dbr = new AMSAReportContext();
                //Get users that have already received an invitation (if they received an invitation this means that they are ready for a reminder
                //Always keep users on the reminder list
                auxLstParticipants = dbr.AMSAParticipant.Where(m => m.Invitation_date != null && m.AMSAEvent.id == idSelectedEvent).ToList();
            }
            if(auxLstParticipants != null) { 
                //Remove all participants that have finished the event... if they have finished the event also update their values just in case
                foreach(AMSAParticipant p in auxLstParticipants)
                {
                    //if the participant has not finished the event then we show it else we don't
                    if (!p.finishedEvent())
                    {
                        lstParticipantForReminder.Add(p);
                    }
                    else
                    {
                        //if the participant has finished check if the status is completed and if not change it to this
                        if(!(p.Status.ToUpper().Trim().Equals("COMPLETED") || p.Status.ToUpper().Trim().Equals("COMPLETE"))){
                            p.Status = "COMPLETED";
                            p.saveChanges();
                        }
                    }
                }
                    //Order the list alphabetically
                    lstParticipantForReminder.OrderBy(m => m.FirstName);
            }
        }


    }
}