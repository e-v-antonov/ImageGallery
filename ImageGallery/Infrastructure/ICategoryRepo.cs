using ImageGallery.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Infrastructure
{
    public interface ICategoryRepo
    {
        IEnumerable<Category> GetAll(string UserName, bool RoleUser);

        Category GetById(int Id);

        void Insert(Category category);

        void Update(Category category);

        void Delete(int id);
    }
}
