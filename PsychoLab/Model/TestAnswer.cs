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
    
    public partial class TestAnswer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TestAnswer()
        {
            this.TestResults = new HashSet<TestResult>();
        }
    
        public int AnswerID { get; set; }
        public Nullable<int> QuestionID { get; set; }
        public string AnswerText { get; set; }
        public Nullable<bool> IsCorrect { get; set; }
    
        public virtual TestQuestion TestQuestion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
