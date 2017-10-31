using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class RoomsPhoto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string photoLink { get; set; }
        [ForeignKey("RoomId")]
        [Newtonsoft.Json.JsonIgnore]
        public virtual Room Room { get; set; }

        public int? RoomId { get; set; }

    }
}