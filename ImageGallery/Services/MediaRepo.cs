using ImageGallery.Data;
using ImageGallery.Infrastructure;
using ImageGallery.Models;
using ImageGallery.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class MediaRepo : IMediaRepo
    {
        private readonly MyContext _context;

        public MediaRepo(MyContext context)
        {
            _context = context;
        }

        public void AddRange(List<Media> model)
        {
            _context.Media.AddRange(model);
        }

        public void Delete(int id)
        {
            var mediaManager = GetById(id);

            _context.Media.Remove(mediaManager);
        }

        public IEnumerable<HomeIndex> GetAll(string UserName, bool RoleUser) //медиа на домашней странице
        {
            if (RoleUser == true)
            {
                var images = _context.Categories.Include(i => i.Media).Select(i => new HomeIndex { Id = i.Id, Title = i.Title, TimeCreateAlbum = i.TimeCreateAlbum, CountImage = i.Media.Count() }).ToList();

                for (int i = 0; i < images.Count; i++)
                {
                    int j = i;
                    images[j].Media = _context.Media.Where(m => m.CategoryId == images[j].Id).Take(4);
                }

                return images;
            }
            else
            {
                var images = _context.Categories.Where(i => i.User.Login == UserName || i.SharedAlbum == true).Include(i => i.Media).Select(i => new HomeIndex { Id = i.Id, Title = i.Title, TimeCreateAlbum = i.TimeCreateAlbum, CountImage = i.Media.Count() }).ToList();

                for (int i = 0; i < images.Count; i++)
                {
                    int j = i;
                    images[j].Media = _context.Media.Where(m => m.CategoryId == images[j].Id).Take(4);
                }

                return images;
            }
        }

        public IEnumerable<Media> GetAllMedia(string UserName, bool RoleUser)   //медиа для администраторов
        {
            if (RoleUser == true)
            {
                return _context.Media.Include(x => x.Category);
            }
            else
            {
                return _context.Media.Include(m => m.Category).Where(m => m.Category.User.Login == UserName || m.Category.SharedAlbum == true );
            }
        }

        public Media GetById(int Id)    //получить данные изображения по id
        {
            return _context.Media.Where(x => x.Id == Id).Include(x => x.Category).FirstOrDefault();
        }

        public void Insert(Media mediaManager)  //добавление изображения в базу данных
        {
            _context.Media.Add(mediaManager);
        }

        public void Update(Media mediaManager)  //изменение данных изображения в БД
        {
            _context.Media.Update(mediaManager);
        }
    }
}
