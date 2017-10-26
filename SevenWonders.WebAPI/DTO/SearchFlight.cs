using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SevenWonders.Models
{
    public class SearchFlight
    {
        public int SelectedSortId { get; set; }
        public List<SelectListItem> SortOptions { get; set; }

        public string Number { get; set; }
        public string Departure { get; set; }
        public string Arrival { get; set; }
        public string Aireplane { get; set; }

        public int Page { get; set; }
        public SearchFlight()
        {
            SortOptions = new List<SelectListItem>();
        }
    }
}