using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [ForeignKey("FlightId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Flight Flight { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public int? FlightId { get; set; }
    }
}