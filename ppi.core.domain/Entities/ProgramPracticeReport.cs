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
    
    public partial class ProgramPracticeReport
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public int PracticeReportId { get; set; }
    
        public virtual PracticeReport PracticeReport { get; set; }
        public virtual Program Program { get; set; }
    }
}
