using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAProgram
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Name for Program is required")]
        public string Name { get; set; }
        public AMSAProgram() { }
    }
}