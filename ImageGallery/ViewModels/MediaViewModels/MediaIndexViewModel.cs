using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.MediaViewModels
{
    public class MediaIndexViewModel
    {
        public List<MediaViewModel> Medias { get; set; }

        public PaginatedList PaginatedList { get; set; }
    }
}
