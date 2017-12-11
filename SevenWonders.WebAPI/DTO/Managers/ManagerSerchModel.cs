using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Managers
{
    public class ManagerSerchModel
    {
      
        public string Name { get; set; }
        public string LastName { get; set; }
        public int AmountOfToursMax { get; set; }
        public int AmountOfToursMin { get; set; }
        public decimal TotalProfitMax { get; set; }
        public decimal TotalProfitMin { get; set; }
        //public string CountryName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}