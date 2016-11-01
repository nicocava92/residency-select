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
        [DisplayName("E-mail Primary E-mail address")]
        public string PrimaryEmail { get; set; }
        [Required (ErrorMessage = "Please insert a Gender")]
        public string Gender { get; set; }
        [Required(ErrorMessage = "Please insert a Title")]
        public string Title { get; set; }
        [Required]
        public AMSAEvent AMSAEvent { get; set; }
        [Required(ErrorMessage = "Please insert a AAMC Number")]
        public string AAMCNumber { get; set; }

        //Same as Hogan Code
        public string AMSACode { get; set; }
        //Password
        public string AMSA_Password { get; set; }

    }
}