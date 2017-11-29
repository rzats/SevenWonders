using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Search
{
    public class TourForBookingModel
    {
        public int PersonAmount { get; set; }
        public DateTime LeaveDate { get; set; }
        public int Duration { get; set; }
        public int RoomId { get; set; }
        public int LeaveScheduleId { get; set; }
        public int ReturnScheduleId { get; set; }
        public bool WithoutFood { get; set; }
    }
}