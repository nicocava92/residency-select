using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.Event.ViewModel
{
    public class AMSASiteViewModel
    {
        public AMSASite AMSASite { get; set; }
        public SelectList lstOrganization { get; set; }
        public int idSelectedOrganization { get; set; }

        public AMSASiteViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            lstOrganization = new SelectList(dbr.AMSAOrganization.ToList(), "id", "Name");
            dbr.Dispose();
        }
        public void loadSelectedData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSASite amsaSite = dbr.AMSASite.Find(id);
            this.idSelectedOrganization = amsaSite.AMSAOrganization.id;
            dbr.Dispose();
        }
        public void saveChanges()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSASite amsaSite = dbr.AMSASite.Find(this.AMSASite.id);
            amsaSite.Name = this.AMSASite.Name;
            AMSAOrganization amsaOrganization = dbr.AMSAOrganization.Find(idSelectedOrganization);
            amsaSite.AMSAOrganization = amsaOrganization;
            dbr.SaveChanges();
        }

    }

}