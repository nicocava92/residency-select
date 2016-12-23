using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSABillingViewModel
    {
        public List<AMSAEvent> LlstAMSAEvents { get; set; }
        public List<AmsaReportStudentData> LstAMSAReports { get; set; }

        //Receives participantId (AMSA CODE) and returns participant
        public AMSAParticipant getParticipantById(string participantID, int eventId)
        {
            AMSAReportContext dbo = new AMSAReportContext();
            AMSAParticipant lstParticipant = dbo.AMSAParticipant.Where(m => m.AMSACode.ToUpper().Equals(participantID.ToUpper()) && m.AMSAEvent.id == eventId).FirstOrDefault();
            dbo.Dispose();
            return lstParticipant;
        }
    }
}