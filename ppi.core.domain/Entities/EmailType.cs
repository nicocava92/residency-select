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
    
    public partial class EmailType
    {
        public EmailType()
        {
            this.Email = new HashSet<Email>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int NameResxId { get; set; }
    
        public virtual ICollection<Email> Email { get; set; }
        public virtual Resx EmailTypeNameText { get; set; }
    }
}
