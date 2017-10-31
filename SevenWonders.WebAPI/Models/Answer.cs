using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public string Text { get; set; }

        [ForeignKey("ManagerId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Manager Manager { get; set; }
        [ForeignKey("QuestionId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Question Question { get; set; }

        [Required]
        public int? ManagerId { get; set; }
        [Required]
        public int? QuestionId { get; set; }

    }
}