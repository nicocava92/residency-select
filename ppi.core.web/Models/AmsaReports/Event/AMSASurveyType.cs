﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSASurveyType
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Name for Survey Type is required")]
        public string Name { get; set; }
    }
}