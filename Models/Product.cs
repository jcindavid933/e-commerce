using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace e_commerce.Models
{
    public class Product
    {
        [Key]
        public int ProductId {get;set;}
        [Required]
        [MinLength(2)]
        public string Name {get;set;}
        [Required]
        public string Desc {get;set;}
        [Required]
        public int Quantity {get;set;}
        [Required]
        [DataType(DataType.Upload)]
        [NotMapped]
        public IFormFile ImageFile{get;set;}
        public string Image {get;set;}
        public DateTime created_at {get;set;} = DateTime.Now;
        public DateTime updated_at {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        [ForeignKey("UserId")]
        public User user {get;set;}
        public List<Order> Order {get;set;}

    }
}