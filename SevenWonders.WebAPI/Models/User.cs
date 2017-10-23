﻿using SevenWonders.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SevenWonders.WebAPI.Models
{
    public class User:IPerson
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [Required]
        public int RoleId { get; set; }
    }
}