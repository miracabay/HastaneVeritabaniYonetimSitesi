//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HastaneVeritabani.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Malzeme_Stok
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Malzeme_Stok()
        {
            this.Malzeme_Alim = new HashSet<Malzeme_Alim>();
            this.Malzeme_Kullanim = new HashSet<Malzeme_Kullanim>();
        }
    
        public int malzeme_id { get; set; }
        public string malzeme_ad { get; set; }
        public int malzeme_adet { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Malzeme_Alim> Malzeme_Alim { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Malzeme_Kullanim> Malzeme_Kullanim { get; set; }
    }
}
