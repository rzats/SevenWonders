using SevenWonders.WebAPI.DTO.Account.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenWonders.WebAPI.Models
{
    public class Customer : ICustomer
    {
       
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public int? CityId { get; set; }
        public decimal TotalPayment { get; set; }
        public int Discount { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public virtual List<Tour> Tours { get; set; }
    }
}