using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.MediaViewModels
{
    public class MediaViewModel
    {
        public int Id { get; set; }

        public string ImagePath { get; set; }

        public string CategoryTitle { get; set; }
    }
}
