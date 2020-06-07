using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.CategoryViewModels
{
    public class CategoryIndexViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }

        public PaginatedList PaginatedList { get; set; }
    }
}
