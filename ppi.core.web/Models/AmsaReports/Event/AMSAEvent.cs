using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAEvent
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //Users should be able to upload and administer event types
        public AMSAEventType AMSAEventType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReviewDate { get; set; }

        //Users should be able to upload and administer event status
        public AMSAEventStatus AMSAEventStatus { get; set; }

        public DateTime CreateDate { get; set; }
        //What is placement is it a boolean or does it reference something !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public bool Placement { get; set; }
        public bool Billable { get; set; }

        public AMSAProgramSite AMSAProgramSite { get; set; }

        public AMSASurveyType AMSASurveyType { get; set; }


    }
}