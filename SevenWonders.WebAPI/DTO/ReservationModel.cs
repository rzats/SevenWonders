
using SevenWonders.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.Models
{
    public class ReservationModel
    {
        public ReservationModel()
        {
            Rooms = new List<Room>();
            TotalPrices = new List<decimal>();
            PricesWithoutFood = new List<decimal>();
            FoodInclude = new List<bool>();
        }
        public decimal MinPrice { get; set; }
        public Reservation Reservation { get; set; }
        public Hotel Hotel { get; set; }
        public List<Room> Rooms { get; set; }
        public List<decimal> TotalPrices { get; set; }
        public List<decimal> PricesWithoutFood { get; set; }
        public int Duration { get; set; }
        public List<bool> FoodInclude { get; set; }
        public bool Food { get; set; }
    }
}