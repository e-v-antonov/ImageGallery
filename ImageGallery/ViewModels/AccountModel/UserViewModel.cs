using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.AccountModel
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public string Surname { get; set; }

        public string Name { get; set; }

        public string Patronymic { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public Role Role { get; set; }
    }
}
