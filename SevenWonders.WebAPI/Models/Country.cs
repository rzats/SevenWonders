using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey("ManagerId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Manager Manager { get; set; }
        public int? ManagerId { get; set; }
    }
}