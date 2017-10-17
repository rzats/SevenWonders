using System;

namespace SevenWonders.Interfaces
{
    public interface IAuthorizedPerson : IPerson
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        bool IsDeleted { get; set; }
        string PhoneNumber { get; set; }
        DateTime DateOfBirth { get; set; }
    }
}
