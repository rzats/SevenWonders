using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Facility
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Newtonsoft.Json.JsonIgnore]

        public virtual List<Hotel> Hotels { get; set; }
    }
}