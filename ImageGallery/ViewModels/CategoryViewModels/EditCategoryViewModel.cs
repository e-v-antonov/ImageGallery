using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.CategoryViewModels
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите название альбома.")]
        [StringLength(50, ErrorMessage = "Длина названия альбома должна быть от 1 до 50 символов.", MinimumLength = 1)]
        public string Title { get; set; }

        public string DescriptionAlbum { get; set; }

        public bool SharedAlbum { get; set; }

        public int UserId { get; set; }
    }
}
