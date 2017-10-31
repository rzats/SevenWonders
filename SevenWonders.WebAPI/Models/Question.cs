using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("CustomerId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Customer Customer { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int? CustomerId { get; set; }
    }
}