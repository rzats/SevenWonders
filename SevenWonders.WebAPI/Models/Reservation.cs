using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Reservation
    { 
        [Key]
        public int Id { get; set; }
        [ForeignKey("RoomId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Room Room { get; set; }
        [Required]
        public int PersonAmount { get; set; }
        [ForeignKey("LeaveFlightId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Flight LeaveFlight { get; set; }
        [ForeignKey("ReturnFlightId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Flight ReturnFlight { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime LeaveDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public bool WithoutFood { get; set; }

        [Required]
        public int? RoomId { get; set; }
        [Required]
        public int? LeaveFlightId { get; set; }
        [Required]
        public int? ReturnFlightId { get; set; }
    }
}