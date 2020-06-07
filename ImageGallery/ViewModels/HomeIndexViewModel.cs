using ImageGallery.Data;
using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<HomeIndex> HomeModel { get; set; }

        public PaginatedList PaginatedList { get; set; }
    }
}
