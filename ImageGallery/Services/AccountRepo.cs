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
    public class AccountRepo : IAccountRepo
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public AccountRepo(MyContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public void Insert(User user)   //добавление пользователя
        {
            bool presenceLogin = _context.Users.Where(u => u.Login == user.Login).Any();    //наличие такого же логина в БД

            if (!presenceLogin)
            {
                _context.Users.Add(user);
            }
            else
            {
                throw new Exception("Данный логин уже существует в системе!");
            }            
        }

        public void Update(User user, string userLogin) //изменение пользователя
        {
            var loginUser = _context.Users.Where(u => u.Login == userLogin).AsNoTracking().FirstOrDefault();     //пользователь, который изменяет данные
            bool presenceLogin = _context.Users.Where(u => u.Login == user.Login && u.Id != user.Id).Any();      //наличие такого же логина в БД

            if (!presenceLogin)
            {
                if (user.Login == userLogin)    //если администратор изменяет данные сам себе
                {
                    if (user.RoleId == loginUser.RoleId)    //если роли совпадают после изменения
                    {
                        _context.Users.Update(user);
                    }
                    else
                    {
                        throw new Exception("Администратор не может изменять роль самому себе.");
                    }
                }
                else
                {
                    _context.Users.Update(user);
                }
            }
            else
            {
                throw new Exception("Данный логин уже существует в системе!");
            }                   
        }

        public void Delete(int id, string Login)    //удаление пользователя
        {
            var deleteUser = GetById(id);
            var albumUser = _context.Categories.Where(c => c.UserId == deleteUser.Id).ToList();
            List<Media> imageUser = new List<Media>();

            for (int i = 0; i < albumUser.Count; i++)   //получаем список изображений удаляемых альбомов
            {
                imageUser.AddRange(_context.Media.Where(m => m.CategoryId == albumUser[i].Id).ToList());
            }

            if (deleteUser.Login != Login)  //админ не удаляет сам себя
            {
                for (int i = 0; i < imageUser.Count(); i++) //удаляем все изображения пользователя
                {
                    string fullPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", imageUser[i].ImagePath);

                    try
                    {
                        File.Delete(fullPath);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Не удалось удалить изображения.");
                    }

                    _context.Media.Remove(imageUser[i]);
                }

                _context.Users.Remove(deleteUser);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception("Администратор не может удалить себя из системы. Попросите удалить вас другого администратора.");
            }
        }

        public IEnumerable<User> GetAll()   //получение списка всех пользователей
        {
            return _context.Users.Include(u => u.Role);
        }

        public User GetById(int Id) //получение данных пользователя по id
        {
            return _context.Users.Where(u => u.Id == Id).Include(u => u.Role).FirstOrDefault();
        }
    }
}
