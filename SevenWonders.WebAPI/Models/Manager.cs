using SevenWonders.WebAPI.DTO.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SevenWonders.WebAPI.Models
{
    public class Manager : IManager
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string Email { get; set; }
        public bool IsDeleted { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Country> Countries { get; set; }
    }
}