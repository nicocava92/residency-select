using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AonParagraphs
    {
        public int id { get; set; }
        public virtual string scale { get; set; }
        public virtual string scale_description { get; set; }
        public virtual string GATEField { get; set; }
        public virtual string paragraph { get; set; }
    }
}