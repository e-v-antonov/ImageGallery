using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.MediaViewModels
{
    public class MediaCreateViewModel
    {
        [Required(ErrorMessage = "Выберите изображение.")]
        public List<IFormFile> Files { get; set; }

        [Required(ErrorMessage = "Изображение обязательно должно находиться в альбоме.")]
        public int CategoryId { get; set; }
    }
}
