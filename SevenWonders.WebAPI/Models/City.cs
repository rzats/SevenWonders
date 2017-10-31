using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class City
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [ForeignKey("CountryId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Country Country { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public int? CountryId { get; set; }
    }
}