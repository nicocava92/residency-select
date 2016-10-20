using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AonItemsAF716AMSAVersionAB
    {
        public int Id { get; set; }
        public virtual string Report { get; set; }
        public virtual string Label { get; set; }
        public virtual string GATEFields { get; set; }
        public virtual string VersionA { get; set; }
        public virtual string VersionB { get; set; }
    }
}