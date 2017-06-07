using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PPI.Core.Web.Models.AmsaReports.Email;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAParticipant
    {
        //FirstName, LastName, PrimaryEmail, PersonId, Identified Gender, Title
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is Required")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please insert a valid e-mail address")]
        [EmailAddress(ErrorMessage = "Please insert a valid e-mail address")]
        [DisplayName("Primary E-mail address")]
        public string PrimaryEmail { get; set; }
        [DisplayName("Identified Gender")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Please insert a Title")]
        public string Title { get; set; }
        [DisplayName("Event")]
        public virtual AMSAEvent AMSAEvent { get; set; }
        [Required(ErrorMessage = "Please insert a AAMC Number")]
        [DisplayName("AAMC Number")]
        public string AAMCNumber { get; set; }

        //Same as Hogan Code
        [DisplayName("GATE ID")]
        [Required(ErrorMessage = "GATE ID is Required")]
        public string AMSACode { get; set; }
        //Password
        [PasswordPropertyText]
        [DisplayName("Password")]
        [Required(ErrorMessage = "Password is Required")]
        public string AMSA_Password { get; set; }


        /*
        Datas for E-mails:
            If Invitation_date is null it means that the user was not invited
            If Reminder_date is null it means that the user was not reminded of their invitation
        */
        public DateTime? Invitation_date { get; set; }
        public DateTime? Reminder_date { get; set; }

        //Status in which the user is (Incomplet,InProcess, Invited) Need to get exact data for here from Sonya
        public string Status { get; set; }

        public string getSurveyType()
        {
            string s = "";
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAParticipant p = dbr.AMSAParticipant.Find(this.Id);
            dbr.Dispose();
            return p.AMSAEvent.AMSASurveyType.Name;
        }

        //When a new participant is created their status should be automatically 
        //set to new and changed when specified (used for listing)
        public AMSAParticipant()
        {
            Status = "NEW";
            Invitation_date = null;
            Reminder_date = null;
        }

        //marks the date when reminder and invitation e-mails where sent
        internal void emailReceived(string type)
        {
            if (type.ToUpper().Equals("INVITATION")) {
                this.Invitation_date = DateTime.Now;
                this.Status = "Invited";
            }
            if (type.ToUpper().Equals("REMINDER"))
            {
                this.Reminder_date = DateTime.Now;
                this.Status = "Reminded";
            }
            this.saveChanges();
        }

        //Saves changes performed to the participant
        internal void saveChanges() {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAParticipant p = dbr.AMSAParticipant.Find(this.Id);
            //set all data for the participant and store
            p.FirstName = this.FirstName;
            p.LastName = this.LastName;
            p.PrimaryEmail = this.PrimaryEmail;
            p.Gender = this.Gender;
            p.Title = this.Title;
            p.AMSAEvent = dbr.AMSAEvent.Find(this.AMSAEvent.id);
            p.AAMCNumber = this.AAMCNumber;
            p.AMSACode = this.AMSACode;
            p.AMSA_Password = this.AMSA_Password;
            p.Invitation_date = this.Invitation_date;
            p.Reminder_date = this.Reminder_date;
            p.Status = this.Status;
            dbr.SaveChanges();
            dbr.Dispose();
        }

        //Checks if reminder e-mail needs to be sent or not
        internal bool timeToSendReminder(AMSAEmail email)
        {
            //Check if the PARTICIPANT HAS FINISHED THEN THERE IS NO POINT IN SENDING OVER A INVITATION NOR REMINDER
            bool finished = participantFinished();
            if (!finished) { 
            if (this.Invitation_date != null)
            {
                //Send initial reminder
                if (this.Reminder_date == null) {
                    DateTime invitation = Invitation_date ?? DateTime.Now;
                    TimeSpan i = (DateTime.Now - invitation);
                    int numOfDays = Convert.ToInt32(i.TotalDays);
                    if (numOfDays >= email.automaticReminderDays && Reminder_date == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            //If initial reminder has been sent then we check against the reminder sent date to see if we need to send a reminder
            else
            {
                DateTime reminder = Reminder_date ?? DateTime.Now;
                TimeSpan i = (DateTime.Now - reminder);
                int numOfDays = Convert.ToInt32(i.TotalDays);
                if (numOfDays >= email.automaticReminderDays)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Invoked to test if a participant has finished or no performing what is required form him in the Event
        /// </summary>
        /// <returns></returns>
        private bool participantFinished()
        {
            return this.Status.ToUpper().Equals("COMPLETED") || this.Status.ToUpper().Equals("COMPLETED PASS") || this.Status.ToUpper().Equals("COMPLETE") || this.Status.ToUpper().Equals("COMPLETE PASS");
        }

        //Checkes if the amsa code exists or doesn't exist
        internal bool amsaCodeExits(int eventId)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(m => m.AMSAEvent.id == eventId && m.Code.Equals(this.AMSACode)).FirstOrDefault();
            dbr.Dispose();
            return c != null && !c.Used;
        }

        /// <summary>
        /// Checks if the user has finished the event
        /// </summary>
        /// <returns>True if the participant has completed the Event, false if the participant hasn't</returns>
        internal bool finishedEvent()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            //Get report 
            AmsaReportStudentData rData = dbr.lstStudentsForReport.Where(m => m.GateID.ToUpper().Equals(this.AMSACode.ToUpper())).FirstOrDefault();
            if (rData == null)
                return false;
            //Return the boolean for the comparison to Completed or Complete in regards to the level of completion of the Event
            return  "COMPLETED" == rData.Status.ToUpper().Trim() ||
                    "COMPLETE" == rData.Status.ToUpper().Trim();
        }
    }

    

    public class AMSAParticipantForCSV { 
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [EmailAddress(ErrorMessage = "Please insert a valid e-mail address")]
        [DisplayName("Primary E-mail address")]
        public string PrimaryEmail { get; set; }
        public string Gender { get; set; }
        public string Title { get; set; }
        [DisplayName("AAMC Number")]
        public string AAMCNumber { get; set; }

        //Same as Hogan Code
        [DisplayName("GATE ID")]
        public string AMSACode { get; set; }
        
    }
}