using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Flight
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Number { get; set; }
        [ForeignKey("AirplaneId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Airplane Airplane { get; set; }
        [ForeignKey("DepartureAirportId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Airport DepartureAirport { get; set; }
        [ForeignKey("ArrivalAirportId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Airport ArrivalAirport { get; set; }
        [Required]
        public decimal Price { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public int? AirplaneId { get; set; }
        [Required]
        public int? DepartureAirportId { get; set; }
        [Required]
        public int? ArrivalAirportId { get; set; }
    }
}