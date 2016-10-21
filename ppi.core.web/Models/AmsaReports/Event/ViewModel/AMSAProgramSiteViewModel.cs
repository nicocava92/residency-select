using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.Event.ViewModel
{
    public class AMSAProgramSiteViewModel
    {
        public AMSAProgramSite AMSAProgramSite{ get; set; }
        public SelectList AMSAProgram { get; set; }
        public SelectList AMSASite { get; set; }
        public int idSelectedProgram { get; set; }
        public int idSelectedSite { get; set; }
    
        public AMSAProgramSiteViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAProgram = new SelectList(dbr.AMSAProgram.ToList(), "id", "Name");
            AMSASite = new SelectList(dbr.AMSASite.ToList(), "id", "Name");
            dbr.Dispose();
        }

        public void loadSelectedData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAProgramSite = dbr.AMSAProgramSite.Find(id);
            idSelectedProgram = AMSAProgramSite.AMSAProgram.id;
            idSelectedSite = AMSAProgramSite.AMSASite.id;
            dbr.Dispose();
        }

        public void saveChanges()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAProgramSite amsaProgramSite = dbr.AMSAProgramSite.Find(this.AMSAProgramSite.id);
            //Get values from the db to store them for program and site
            amsaProgramSite.AMSAProgram = dbr.AMSAProgram.Find(this.idSelectedProgram);
            amsaProgramSite.AMSASite = dbr.AMSASite.Find(this.idSelectedSite);
            dbr.SaveChanges();
            dbr.Dispose();
        }

        public void saveNewSite()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAProgramSite amsaProgramSite = new AMSAProgramSite();
            amsaProgramSite.AMSAProgram = dbr.AMSAProgram.Find(this.idSelectedProgram);
            amsaProgramSite.AMSASite = dbr.AMSASite.Find(this.idSelectedSite);
            dbr.AMSAProgramSite.Add(amsaProgramSite);
            dbr.SaveChanges();
            dbr.Dispose();
        }

    }
}