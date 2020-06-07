using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Infrastructure
{
    public interface IAccountRepo
    {
        IEnumerable<User> GetAll();

        User GetById(int Id);

        void Insert(User user);

        void Update(User user, string userLogin);

        void Delete(int id, string Login);
    }
}
