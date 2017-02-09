using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AmsaReportStudentData
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [System.ComponentModel.DisplayName("First Name")]
        public virtual string FirstName { get; set; }
        [System.ComponentModel.DisplayName("Last Name")]
        [Required(ErrorMessage = "Last Name is Required")]
        public virtual string LastName { get; set; }
        [System.ComponentModel.DisplayName("Person Id")]
        [Required(ErrorMessage = "Person Id is Required")]
        public virtual string PersonId { get; set; }
        //Registration date
        [System.ComponentModel.DisplayName("Registration Date")]
        public virtual DateTime RegistrationDate { get; set; }
        [System.ComponentModel.DisplayName("Completion Date")]
        public virtual DateTime CompletionDate { get; set; }
        //Gate id == AMSA CODE
        public virtual String GateID { get; set; }
        //Ambition
        [System.ComponentModel.DisplayName("Stanine Ambition")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Ambition { get; set; }
        //Assertiveness
        [System.ComponentModel.DisplayName("Stanine Assertiveness")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Assertiveness { get; set; }
        //Awareness- Stanine_Awareness
        [System.ComponentModel.DisplayName("Stanine Awareness")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Awareness { get; set; }
        //Composure - Self-Control
        [System.ComponentModel.DisplayName("Stanine Composure")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Composure { get; set; }
        //Conceptual - Don't know
        [System.ComponentModel.DisplayName("Stanine Conceptual")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Conceptual { get; set; }
        //Team Orientation - Group Focus
        [System.ComponentModel.DisplayName("Stanine Cooperativeness")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Cooperativeness { get; set; }
        //Drive - Motivational
        [System.ComponentModel.DisplayName("Stanine Drive")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Drive { get; set; }
        //Open-Mindedness
        [System.ComponentModel.DisplayName("Stanine Flexibility")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Flexibility { get; set; }
        //Humility
        [System.ComponentModel.DisplayName("Stanine Humility")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Humility { get; set; }
        //Approachability
        [System.ComponentModel.DisplayName("Stanine Liveliness")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Liveliness { get; set; }

        //Mastery
        [System.ComponentModel.DisplayName("Stanine Mastery")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Mastery { get; set; }
        //Positivity
        [System.ComponentModel.DisplayName("Stanine Positivity")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Positivity { get; set; }
        //Control
        [System.ComponentModel.DisplayName("Stanine Power")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Power { get; set; }
        //Compassion
        [System.ComponentModel.DisplayName("Stanine Sensitivity")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Sensitivity { get; set; }
        //Organization
        [System.ComponentModel.DisplayName("Stanine Structure")]
        [Range(1, 9, ErrorMessage = "Must be between 1 and 9")]
        public virtual int Stanine_Structure { get; set; }

        //Current status
        public string Status { get; set; }

        /****************
        AMSA Event that is related to the data    
        ****************/
        public virtual AMSAEvent AMSAEvent { get; set; }

        //Date that the report is stored into the database
        public DateTime? Updated { get; set; }

        public AmsaReportStudentData()
        {
            Updated = DateTime.Now;
            Status = "Not Started";
        }

        //Method used in profile section of the application
        //Get Program from a Data
        public AMSAProgram getProgram()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent e = dbr.AMSAEvent.Find(this.AMSAEvent.id);
            AMSAProgram p = dbr.AMSAProgram.Find(e.AMSAProgram.id);
            dbr.Dispose();
            return p;
        }
        
    }
}