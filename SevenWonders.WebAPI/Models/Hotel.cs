using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("RatingId")]
        public virtual Rating Rating { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public string Description { get; set; }
        [ForeignKey("FoodTypeId")]
        public virtual FoodType FoodType { get; set; }
        [Required]
        public string Adress { get; set; }
        [Required]
        public decimal FoodPrice { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public int? RatingId { get; set; }
        [Required]
        public int? CityId { get; set; }
        [Required]
        public int? FoodTypeId { get; set; }
        public virtual List<Feedback> Feedbacks { get; set; }
        public virtual List<Facility> Facilities { get; set; }
        public virtual List<HotelsPhoto> HotelsPhotos { get; set; }
    }
}