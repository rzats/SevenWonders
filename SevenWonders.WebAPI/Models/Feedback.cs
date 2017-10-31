using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenWonders.WebAPI.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CustomerId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Customer Customer { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public string Text { get; set; }
        [ForeignKey("HotelId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Hotel Hotel { get; set; }
        [ForeignKey("RatingId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Rating Rating { get; set; }

        [Required]
        public int? CustomerId { get; set; }
        [Required]
        public int? HotelId { get; set; }
        [Required]
        public int? RatingId { get; set; }
    }
}