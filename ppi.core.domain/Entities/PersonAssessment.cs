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
    
    public partial class PersonAssessment
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public Nullable<int> AssessmentId { get; set; }
        public bool AssessmentComplete { get; set; }
    }
}
