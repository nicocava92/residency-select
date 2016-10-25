using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PPI.Core.Web.Models.AmsaReports.Event;
using System.ComponentModel.DataAnnotations;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAEvent
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please enter a name for the event")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a description for the event")]
        public string Description { get; set; } 
        //Users should be able to upload and administer event types
        [Required(ErrorMessage = "Please select an event type")]
        public AMSAEventType AMSAEventType { get; set; }  //Event type
        
        /*******
        Need to check how to add errors here
        *********/

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public DateTime CreateDate { get; set; }
        //If Composite = true
        public DateTime CompositeNeedByDate { get; set; }
        //If JetNeedByDate = true
        public DateTime JetNeedByDate { get; set; }


        //Users should be able to upload and administer event status
        
        public AMSAEventStatus AMSAEventStatus { get; set; }

        
        
        public int Placement { get; set; }
        public bool Billable { get; set; } 

        public AMSAOrganization AMSAOrganization { get; set; } //Organization

        public AMSASurveyType AMSASurveyType { get; set; } // Survey Type

        public AMSAProgram AMSAProgram { get; set; } // Speciality

        public AMSASite AMSASite { get; set; } //Departement

        
        public int TotalNumberOfParticipants { get; set; } 

        public string defaultEmailAddress { get; set; } 

        public string OrderBy { get; set; } 

        public bool JetRequired { get; set; }
        public bool CompositeRequired { get; set; }

        //If not billable select the reason
        public AMSANotBillableReason AMSANotBillableReason { get; set; }

        //If an event is not billable change the menu and expand information to add in why the event is not billable


        //Events start with a value of not started
        

    }
}