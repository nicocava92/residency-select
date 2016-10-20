using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAProgramSite
    {
        public int id { get; set; }
        public AMSASite AMSASite { get; set; }
        public AMSAProgram AMSAProgram { get; set; }
    }

}