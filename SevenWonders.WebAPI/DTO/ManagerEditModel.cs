﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SevenWonders.WebAPI.DTO
{
    public class ManagerEditModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public List<DropDownListItem> Countries { get; set; }

        public ManagerEditModel()
        {
            Countries = new List<DropDownListItem>();
        }
    }
}