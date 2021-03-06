//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PPI.Core.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class PracticeReport
    {
        public PracticeReport()
        {
            this.EventPracticeReport = new HashSet<EventPracticeReport>();
            this.OrderFormPracticeReport = new HashSet<OrderFormPracticeReport>();
            this.PersonPracticeReport = new HashSet<PersonPracticeReport>();
            this.PracticeParagraphs = new HashSet<PracticeParagraphs>();
            this.PracticeScaleReport = new HashSet<PracticeScaleReport>();
            this.ProgramPracticeReports = new HashSet<ProgramPracticeReports>();
            this.OESPracticeReport = new HashSet<OESPracticeReport>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReportTitle { get; set; }
        public Nullable<int> ReportTitleResxId { get; set; }
        public string PracticeGroup { get; set; }
        public byte[] DefaultLogo { get; set; }
        public string DefaultLogoMimeType { get; set; }
        public byte[] DefaultBackground { get; set; }
        public string DefaultBackgroundMimeType { get; set; }
        public string DefaultColor { get; set; }
        public string Introduction { get; set; }
        public Nullable<int> IntroductionResxId { get; set; }
        public string IntroductionTwo { get; set; }
        public Nullable<int> IntroductionTwoResxId { get; set; }
        public string IntroductionThree { get; set; }
        public Nullable<int> IntroductionThreeResxId { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual ICollection<EventPracticeReport> EventPracticeReport { get; set; }
        public virtual ICollection<OrderFormPracticeReport> OrderFormPracticeReport { get; set; }
        public virtual ICollection<PersonPracticeReport> PersonPracticeReport { get; set; }
        public virtual ICollection<PracticeParagraphs> PracticeParagraphs { get; set; }
        public virtual Resx IntroductionResx { get; set; }
        public virtual Resx IntroductionThreeResx { get; set; }
        public virtual Resx IntroductionTwoResx { get; set; }
        public virtual Resx ReportTitleResx { get; set; }
        public virtual ICollection<PracticeScaleReport> PracticeScaleReport { get; set; }
        public virtual ICollection<ProgramPracticeReports> ProgramPracticeReports { get; set; }
        public virtual ICollection<OESPracticeReport> OESPracticeReport { get; set; }
    }
}
