using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace e_commerce.Models
{
    public class Order
    {
        [Key]
        public int OrderId {get;set;}
        [Required]
        public int Quantity {get;set;}
        public DateTime created_at {get;set;} = DateTime.Now;
        public DateTime updated_at {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        [Required]
        [ForeignKey("UserId")]
        public User user {get;set;}
        public int ProductId {get;set;}
        [Required]
        [ForeignKey("ProductId")]
        public Product product {get;set;}

    }
}