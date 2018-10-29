using System;
using System.ComponentModel.DataAnnotations;

namespace e_commerce.Models
{
    public class Login
    {
        [Key]
        public int id {get;set;}
        [Required]
        [EmailAddress]
        public string Login_Email{get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string Login_Password{get;set;}

    }
}