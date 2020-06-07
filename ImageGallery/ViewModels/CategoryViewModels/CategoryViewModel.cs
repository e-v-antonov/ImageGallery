using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.CategoryViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        public DateTime TimeCreateAlbum { get; set; }

        public string DescriptionAlbum { get; set; }

        public bool SharedAlbum { get; set; }
    }
}
