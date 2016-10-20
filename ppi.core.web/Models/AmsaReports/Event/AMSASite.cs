using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSASite
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public string BrandingColor { get; set; }
        public string BrandingLogo { get; set; }
        public string BrandingLogoMimeType { get; set; }
        public string BrandingBackground { get; set; }
        public string BrandingBackgroundMimeType { get; set; }
        public AMSAOrganization AMSAOrganization { get; set; }
    }
}