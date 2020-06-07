using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Data
{
    public class HomeIndex
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime TimeCreateAlbum { get; set; }

        public IQueryable<Media> Media { get; set; }

        public int CountImage { get; set; }
    }
}
