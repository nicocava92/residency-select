using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSAProgramSite
    {
        public int id { get; set; }
        public virtual AMSASite AMSASite { get; set; }
        public virtual AMSAProgram AMSAProgram { get; set; }
    }

}