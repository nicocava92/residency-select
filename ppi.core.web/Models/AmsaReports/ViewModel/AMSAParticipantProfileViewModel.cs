using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSAParticipantProfileViewModel
    {
        public AMSAParticipant AmsaParticipant { get; set; }
        public List<AMSAProgram> AmsaProgram { get; set; }

        public List<AmsaReportStudentData> AmsaStudentData { get; set; }
        //Still need to add in code relating participant profile and E-mails!!


        //Load this with the Id of the participant, any time this class is called it needs to receive 
        //the id of the participant
        public AMSAParticipantProfileViewModel(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AmsaParticipant = dbr.AMSAParticipant.Where(r => r.Id == id).FirstOrDefault();
            //Find the Program(s) participants are assigned to
            AmsaProgram = this.getListPrograms(dbr,id);
            //Need to get first data Result
            AmsaStudentData = dbr.lstStudentsForReport.Where(r => r.PersonId == AmsaParticipant.AMSACode).OrderByDescending(r=>r.Updated).ToList();
        }

        //Get programs a participant is assigned to
        //To do this we need to get the list of events a participant is assigned to and then return the list of programs
        internal List<AMSAProgram> getListPrograms(AMSAReportContext dbr, int participantId)
        {
            List<AMSAProgram> lstProgram = new List<AMSAProgram>();
            List<AMSAParticipant> lstParticipant = dbr.AMSAParticipant.Where(r => r.Id == participantId).ToList();
            //Loop through participants and get programs from events
            foreach (AMSAParticipant p in lstParticipant)
            {
                lstProgram.Add(p.AMSAEvent.AMSAProgram);
            }
            return lstProgram;
        }

    }
}