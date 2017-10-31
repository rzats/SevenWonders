using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Airport
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("CityId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual City City { get; set; }
        [Required]
        public int? CityId { get; set; }
        public bool IsDeleted { get; set; }

    }
}