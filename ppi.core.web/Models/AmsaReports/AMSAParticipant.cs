using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAParticipant
    {
        //FirstName, LastName, PrimaryEmail, PersonId, Gender, Title
        public int Id { get; set; }
        [Required (ErrorMessage = "First Name is Required")]
        [DisplayName ("First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required (ErrorMessage = "Please insert a valid e-mail address")]
        [EmailAddress(ErrorMessage = "Please insert a valid e-mail address")]
        [DisplayName("Primary E-mail address")]
        public string PrimaryEmail { get; set; }
        public string Gender { get; set; }
        [Required(ErrorMessage = "Please insert a Title")]
        public string Title { get; set; }
        [DisplayName("Event")]
        public virtual AMSAEvent AMSAEvent { get; set; }
        [Required(ErrorMessage = "Please insert a AAMC Number")]
        [DisplayName("AAMC Number")]
        public string AAMCNumber { get; set; }

        //Same as Hogan Code
        [DisplayName("AMSA Code")]
        [Required (ErrorMessage = "AMSA Code is Required")]
        public string AMSACode { get; set; }
        //Password
        [PasswordPropertyText]
        [DisplayName("Password")]
        [Required (ErrorMessage = "Password is Required")]
        public string AMSA_Password { get; set; }

    }
}