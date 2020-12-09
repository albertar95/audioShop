//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AudioShopBackend.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ship
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ship()
        {
            this.Orders = new HashSet<Order>();
        }
    
        public System.Guid NidShip { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public Nullable<byte> ShipVia { get; set; }
        public string Address { get; set; }
        public Nullable<decimal> ZipCode { get; set; }
        public byte State { get; set; }
        public Nullable<decimal> ShipPrice { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
