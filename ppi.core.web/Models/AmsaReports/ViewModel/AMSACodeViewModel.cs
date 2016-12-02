using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.ViewModel
{
    public class AMSACodeViewModel
    {
        public AMSACode AMSACode { get; set; }
        public SelectList Events { get; set; }
        public int idSelectedEvent { get; set; }

        public AMSACodeViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            Events = new SelectList(dbr.AMSAEvent.ToList(), "id", "Name");
            dbr.Dispose();
        }

        public void loadSelectedData(int id)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            this.AMSACode = dbr.AMSACodes.Find(id);
            this.idSelectedEvent = this.AMSACode.Id;
            dbr.Dispose();
        }
        public void saveChanges()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSAEvent e = dbr.AMSAEvent.Find(this.idSelectedEvent);
            AMSACode ac = dbr.AMSACodes.Find(this.AMSACode.Id);
            ac.AMSAEvent = e;
            ac.Code = this.AMSACode.Code;
            ac.Used = this.AMSACode.Used;
            ac.Pin = this.AMSACode.Pin;
            dbr.SaveChanges();
            dbr.Dispose();
        }
        public void saveNewCode()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode ac = new AMSACode();
            ac.Code = this.AMSACode.Code;
            ac.Used = false;
            ac.Pin = this.AMSACode.Pin;
            AMSAEvent e = dbr.AMSAEvent.Find(this.idSelectedEvent);
            ac.AMSAEvent = e;
            dbr.AMSACodes.Add(ac);
            dbr.SaveChanges();
            dbr.Dispose();
        }

        public void checkIfCodeExists(ModelStateDictionary m)
        {
            AMSAReportContext dbr = new AMSAReportContext();
            AMSACode c = dbr.AMSACodes.Where(r => r.Code.ToUpper().Equals(this.AMSACode.Code.ToUpper())).FirstOrDefault();

            if(c != null)
            {
                m.AddModelError("AMSACode", "AMSA Code already exists, please insert a non-existing AMSA Code");
            }
            dbr.Dispose();
        }

    }
}