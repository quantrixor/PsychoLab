//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PsychoLab.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class SessionArchive
    {
        public int ArchiveID { get; set; }
        public System.DateTime ArchivedDate { get; set; }
    
        public virtual Session Session { get; set; }
    }
}
