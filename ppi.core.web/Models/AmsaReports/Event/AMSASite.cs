using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PPI.Core.Web.Models.AmsaReports
{
    public class AMSASite
    {
        public int id { get; set; }
        [Required (ErrorMessage = "Name for AMSA Site is required")]
        public string Name { get; set; }
        [Required (ErrorMessage = "Friendly name for AMSA Site is required")]
        public string FriendlyName { get; set; }
        //public string BrandingColor { get; set; }
        //public string BrandingLogo { get; set; }
        //public string BrandingLogoMimeType { get; set; }
        //public string BrandingBackground { get; set; }
        //public string BrandingBackgroundMimeType { get; set; }
        public virtual AMSAOrganization AMSAOrganization { get; set; }
        public AMSASite() { }
    }
}