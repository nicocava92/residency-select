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
    
    public partial class EventNotBillableReason
    {
        public EventNotBillableReason()
        {
            this.OESSetup = new HashSet<OESSetup>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<OESSetup> OESSetup { get; set; }
    }
}
