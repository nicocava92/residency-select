using System;
using System.Collections.Generic;
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

        /****************
        AMSA Event that is related to the data    
        ****************/
        public AMSAEvent AMSAEvent { get; set; }

    }
}