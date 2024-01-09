﻿using System.ComponentModel.DataAnnotations;

namespace RMall.Models.GalleryMalls
{
    public class CreateGalleryMall
    {
        [Required(ErrorMessage = "Please enter image")]
        public IFormFile imagePath { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        [MinLength(3, ErrorMessage = "Enter at least 3 characters")]
        [MaxLength(255, ErrorMessage = "Enter up to 255 characters")]
        public string productName { get; set; }

        public string? description { get; set; }
    }
}
