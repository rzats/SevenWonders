using SevenWonders.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SevenWonders.Models
{
    public class SearchModel
    {       
        public IEnumerable<SelectListItem> Countries { get; set; }
        public IEnumerable<SelectListItem> FoodTypes { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public int CountryFrom { get; set; }
        public int CityFrom { get; set; }
        public int CountryTo { get; set; }
        public int CityTo { get; set; }
        public int Hotel { get; set; }
        public int FoodType { get; set; }
        public DateTime DapartureDay { get; set; }
        public int Duration { get; set; }
        public int PeopleNumber { get; set; }
        //public List<SelectListItem> Facilities { get; set; }
        //public List<SelectListItem> Stars { get; set; }
        public int PriceFrom { get; set; }
        public int PriceTo { get; set; }
        public string NewMessage { get; set; }
    }
}