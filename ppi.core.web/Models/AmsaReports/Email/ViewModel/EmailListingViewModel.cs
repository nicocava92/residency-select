using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PPI.Core.Web.Models.AmsaReports.Email.ViewModel
{
    public class EmailListingViewModel
    {
        private int? id;

        public IEnumerable<AMSAEmail> lstEmails { get; set; }
        public int idSelectedEvent { get; set; }
        public SelectList Events { get; set; }
        public List<AMSAEvent> lstEvents { get; set; }

        public EmailListingViewModel()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            lstEmails = dbr.AMSAEmail.ToList();
            getListEvents(dbr);
            dbr.Dispose();
        }
        
        private void getListEvents(AMSAReportContext dbr)
        {
            lstEvents = dbr.AMSAEvent.ToList();
            lstEvents.Insert(0, new AMSAEvent { id = 0, Name = " Show All " });
            this.Events = new SelectList(lstEvents, "id", "Name");
        }

        internal void changeEvent()
        {
            AMSAReportContext dbr = new AMSAReportContext();
            this.lstEmails = dbr.AMSAEmail.Where(m => m.AMSAEvent.id == this.idSelectedEvent).ToList();
        }
    }
}