using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAEventStatus
    {
        public int id { get; set; }
        [Required (ErrorMessage = "Name for Event Status is required")]
        public string Name { get; set; }
    }
}