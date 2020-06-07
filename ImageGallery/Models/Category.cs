using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{dd.MM.yyyy}")]
        [DataType(DataType.Date)]
        public DateTime TimeCreateAlbum { get; set; }

        public string DescriptionAlbum { get; set; }

        public bool SharedAlbum { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public List<Media> Media { get; set; } = new List<Media>();
    }
}
