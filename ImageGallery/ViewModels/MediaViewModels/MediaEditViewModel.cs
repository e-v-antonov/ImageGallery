using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.MediaViewModels
{
    public class MediaEditViewModel
    {
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public IFormFile File { get; set; }

        [Required(ErrorMessage = "Изображение обязательно должно находиться в альбоме.")]
        public int CategoryId { get; set; }
    }
}
