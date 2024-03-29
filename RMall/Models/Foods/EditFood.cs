﻿using System.ComponentModel.DataAnnotations;

namespace RMall.Models.Foods
{
    public class EditFood
    {
        [Required] 
        public int id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string name { get; set; }

        public IFormFile? image { get; set; }

        [Required(ErrorMessage = "Please enter price")]
        [Range(0, double.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "Please enter quantity")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a valid decimal value.")]
        public int quantity { get; set; }

        [Required(ErrorMessage = "Please enter description")]
        public string description { get; set; }
    }
}
