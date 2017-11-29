using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO.Shared
{
    public class DropDownListItem
    {
        public string Id { get;set;}
        public string Text { get; set; }
        public bool IsChecked { get; set; }
    }
}