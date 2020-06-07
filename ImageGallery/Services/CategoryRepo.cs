using ImageGallery.Data;
using ImageGallery.Infrastructure;
using ImageGallery.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class CategoryRepo : ICategoryRepo
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public CategoryRepo(MyContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public void Delete(int id)  //удаление альбома
        {
            var category = GetById(id);

            var imagesInAlbum = _context.Media.Where(x => x.CategoryId == category.Id).ToList();

            for (int i = 0; i < imagesInAlbum.Count(); i++) //удаление изображений в альбоме
            {                
                string fullPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", imagesInAlbum[i].ImagePath);

                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception e)
                {
                    throw new Exception("Не удалось удалить изображения.");
                }

                _context.Media.Remove(imagesInAlbum[i]);
            }
           
            _context.Categories.Remove(category);
            _context.SaveChanges();
        }

        public IEnumerable<Category> GetAll(string UserName, bool RoleUser) //получение списках всех альбомов
        {
            if (RoleUser == true)
            {
                return _context.Categories;
            }
            else
            {
                return _context.Categories.Where(x => x.User.Login == UserName || x.SharedAlbum == true);
            }
            
        }

        public Category GetById(int Id) //получение альбома по id
        {
            return _context.Categories.Where(x => x.Id == Id).FirstOrDefault();
        }

        public void Insert(Category category)   //добавление альбома
        {
            if (!SearchIdenticalTitlle(category))
            {
                throw new Exception("Альбом с таким названием уже существует.");
            }                
            else
            {
                _context.Categories.Add(category);
            }                
        }

        public void Update(Category category)   //изменение альбома
        {
            if (!SearchIdenticalTitlle(category))
            {
                throw new Exception("Альбом с таким иназванием уже существует.");
            }                
            else
            {
                _context.Categories.Update(category);
            }                
        }

        private bool SearchIdenticalTitlle(Category newCategory)    //поиск альбома с одинаковым названием
        {
            int idCategory = newCategory.Id;
            var allCategories = _context.Categories.AsNoTracking().ToList();

            for (int i = 0; i < allCategories.Count; i++)
            {
                if (allCategories[i].Title == newCategory.Title && allCategories[i].Id != idCategory)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
