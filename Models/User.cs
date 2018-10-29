using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace e_commerce.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}
        [Required]
        [MinLength(2)]
        public string name {get;set;}
        [Required]
        [MinLength(2)]
        public string alias {get;set;}
        [Required]
        [EmailAddress]
        public string email {get;set;}
        [Required]
        [DataType(DataType.Password)]
        public string password {get;set;}
        [NotMapped]
        [Required]
        [Compare(nameof(password), ErrorMessage = "Passwords don't match.")]
        public string pass_confirm{get;set;}
        public DateTime created_at {get;set;} = DateTime.Now;
        public DateTime updated_at {get;set;} = DateTime.Now;
        public List<Product> Product {get;set;}
        public List<Order> Order {get;set;}

    }
}