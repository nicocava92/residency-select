using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models
{
    public class EmailTemplateModel
    {
        public string subject { get; set; }
        public string introduction { get; set; }
        public string closing { get; set; }

        //For Hogan Report
        public string hogan_Id { get; set; }
        public string hogan_password { get; set; }
        //For AMSA Report
        public string amsa_id { get; set; }
        public string amsa_password { get; set; }


        public string j3p_URL { get; set; }
        public string UserName { get; set; }
        public string EventName { get; set; }
    }
}