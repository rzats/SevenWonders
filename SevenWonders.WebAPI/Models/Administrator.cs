using SevenWonders.WebAPI.DTO.Account.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SevenWonders.WebAPI.Models
{
    public class Administrator :IAuthorizedPerson
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public bool IsDeleted { get; set; }
    }
}