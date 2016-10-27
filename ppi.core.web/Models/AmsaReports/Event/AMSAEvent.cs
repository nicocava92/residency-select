using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PPI.Core.Web.Models.AmsaReports.Event;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAEvent
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Please enter a name for the event")]
        [DisplayName("Event Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please enter a description for the event")]
        [DisplayName("Event Description")]
        public string Description { get; set; } 
        //Users should be able to upload and administer event types
        [DisplayName("Event Type")]
        public virtual AMSAEventType AMSAEventType { get; set; }  //Event type
        
        /*******
        Need to check how to add errors here
        *********/
        [DisplayName("Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayName("Event / Review Date")]
        public DateTime ReviewDate { get; set; }
        [DisplayName("Create Date")]
        public DateTime CreateDate { get; set; }
        [DisplayName("End Date")]
        public DateTime EndDate { get; set; }
        //If Composite = true
        [DisplayName("Composite Needed By Date")]
        public DateTime CompositeNeedByDate { get; set; }
        //If JetNeedByDate = true
        [DisplayName("Jet Needed By Date")]
        public DateTime JetNeedByDate { get; set; }


        //Users should be able to upload and administer event status
        [DisplayName("Event Status")]
        public virtual AMSAEventStatus AMSAEventStatus { get; set; }

        public int Placement { get; set; }
        public bool Billable { get; set; } 

        [DisplayName("Organization")]
        public AMSAOrganization AMSAOrganization { get; set; } //Organization

        [DisplayName("Survey Type")]
        public virtual AMSASurveyType AMSASurveyType { get; set; } // Survey Type

        [DisplayName("Speciality")]
        public AMSAProgram AMSAProgram { get; set; } // Speciality

        [DisplayName("Department")]
        public AMSASite AMSASite { get; set; } //Departement

        [DisplayName("Total Number of Participants")]
        [Required(ErrorMessage = "Please insert number of participants")]
        public int TotalNumberOfParticipants { get; set; } 

        [DisplayName("Default E-mail Address")]
        [EmailAddress(ErrorMessage = "Invalid E-mail Address")]
        [Required(ErrorMessage = "Please enter a Valid E-mail Address")]
        public string defaultEmailAddress { get; set; } 

        [DisplayName("Order By")]
        
        public string OrderBy { get; set; } 

        [DisplayName("Is Jet Required?")]
        public bool JetRequired { get; set; }
        [DisplayName("Composite Required?")]
        public bool CompositeRequired { get; set; }

        //If not billable select the reason
        [DisplayName("Not billable reason")]
        public AMSANotBillableReason AMSANotBillableReason { get; set; }

        //If an event is not billable change the menu and expand information to add in why the event is not billable


        //Events start with a value of not started
        

    }
}