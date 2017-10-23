using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Airplane
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public int SeatsAmount { get; set; }
        public bool IsDeleted { get; set; }
    }
}