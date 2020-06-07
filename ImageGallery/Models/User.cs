using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Name { get; set; }

        public string Patronymic { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(16)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public List<Category> Category { get; set; } = new List<Category>();
    }
}
