//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SevenWonders.WebAPI.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExtraPrice
    {
        public int Id { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int AdditionalPercent { get; set; }
        public bool IsDeleted { get; set; }
        public int HotelId { get; set; }
    
        public virtual Hotel Hotel { get; set; }
    }
}
