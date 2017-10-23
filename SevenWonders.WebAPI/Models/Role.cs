using System.ComponentModel.DataAnnotations;

namespace SevenWonders.WebAPI.Models
{
    public class Role
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}