using ImageGallery.Data;
using ImageGallery.Models;
using ImageGallery.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Infrastructure
{
    public interface IMediaRepo
    {
        IEnumerable<Media> GetAllMedia(string UserName, bool RoleUser);

        IEnumerable<HomeIndex> GetAll(string UserName, bool RoleUser);

        Media GetById(int Id);

        void Insert(Media mediaManager);

        void Update(Media mediaManager);

        void Delete(int id);

        void AddRange(List<Media> model);
    }
}
