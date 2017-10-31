using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class ExtraPrice
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("HotelId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Hotel Hotel { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required]
        public int AdditionalPercent { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public int? HotelId { get; set; }
    }
}