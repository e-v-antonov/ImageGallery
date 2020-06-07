using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.ViewModels.AccountModel
{
    public class UserIndexViewModel
    {
        public List<UserViewModel> Users { get; set; }

        public PaginatedList PaginatedList { get; set; }
    }
}
