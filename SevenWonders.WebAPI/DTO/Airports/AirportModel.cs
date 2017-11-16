using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Airports
{
    public class AirportModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
    }
}