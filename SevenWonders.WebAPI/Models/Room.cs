using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("HotelId")]
        public virtual Hotel Hotel { get; set; }
        [ForeignKey("RoomTypeId")]
        public virtual RoomType RoomType { get; set; }
        [Required]
        public int MaxPeople { get; set; }
        public string WindowView { get; set; }
        [Required]
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }

        public virtual List<Equipment> Equipments { get; set; }
        [Required]
        public int? HotelId { get; set; }
        [Required]
        public int? RoomTypeId { get; set; }
        public virtual List<RoomsPhoto> RoomsPhotos { get; set; }
    }
}