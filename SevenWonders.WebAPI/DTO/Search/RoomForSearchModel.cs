using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Rooms
{
    public class RoomForSearchModel
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public int MaxPeople { get; set; }
        public string WindowView { get; set; }
        public decimal Price { get; set; }
        public decimal PriceWirhFood { get; set; }
        public List<string> Equipments { get; set; }
    }
}