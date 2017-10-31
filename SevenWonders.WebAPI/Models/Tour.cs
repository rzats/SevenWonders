using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ReservationId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Reservation Reservation { get; set; }
        [ForeignKey("CustomerId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Customer Customer { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public decimal? TotalPrice { get; set; }

        [ForeignKey("TourStateId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual TourState TourState { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public int? ReservationId { get; set; }
        [Required]
        public int? CustomerId { get; set; }
        [Required]
        public int? TourStateId { get; set; }
    }
}