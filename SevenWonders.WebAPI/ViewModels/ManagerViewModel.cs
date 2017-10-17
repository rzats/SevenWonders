using System;
using System.ComponentModel.DataAnnotations;

namespace SevenWonders.ViewModels
{
    public class ManagerViewModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

    }
}