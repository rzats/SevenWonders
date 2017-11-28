using System;
using System.ComponentModel.DataAnnotations;

namespace SevenWonders.WebAPI.DTO.Account
{
    public class FullManagerViewModel
    {
        public FullManagerViewModel(int id, string email, string password, string firstName, string lastName, string phoneNumber, DateTime dateOfBirth)
        {
            Id = id;
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }
        public FullManagerViewModel()
        { }
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        // public SelectList CountryListModel { get; set; }
    }
}