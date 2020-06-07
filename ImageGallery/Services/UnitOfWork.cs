using ImageGallery.Data;
using ImageGallery.Infrastructure;
using ImageGallery.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MyContext _context;
        private ICategoryRepo _categoryRepo;
        private IMediaRepo _mediaRepo;
        private readonly IWebHostEnvironment hostingEnvironment;
        private int numberFile = 0;

        public UnitOfWork(MyContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public ICategoryRepo CategoryRepo
        {
            get
            {
                return _categoryRepo = _categoryRepo ?? new CategoryRepo(_context, hostingEnvironment);
            }
        }

        public IMediaRepo MediaRepo
        {
            get
            {
                return _mediaRepo = _mediaRepo ?? new MediaRepo(_context);
            }
        }

        public void Save()  //сохранение изменений в БД
        {
            _context.SaveChanges();
        }

        public void UploadFile(List<IFormFile> files, List <Media> medias)  //загрзука изображений
        {
            foreach (IFormFile item in files)
            {
                long totalBytes = files.Sum(f => f.Length); //узнаем размер изображения в байтах
                string fileName = item.FileName.Trim('"');
                fileName = EnsureFileName(fileName);
                byte[] buffer = new byte[16 * 1024];

                using (FileStream output = File.Create(GetPathAndFileName(medias[numberFile].ImagePath)))
                {
                    using (Stream input = item.OpenReadStream())
                    {
                        int readBytes;

                        while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, readBytes);
                            totalBytes += readBytes;
                        }
                    }
                }
                numberFile++;
            }
        }

        private string EnsureFileName(string fileName)  //возврат имени файла
        {
            if (fileName.Contains("\\"))
            {
                fileName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
            }

            return fileName;
        }

        private string GetPathAndFileName(string FileName)  //формирование полного пути
        {            
            string path = hostingEnvironment.WebRootPath + "\\uploads\\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path + FileName;
        }

        public List<Media> CheckFileName(List<Media> medias)    //проверка одинаковых имен изображений в БД
        {
            for (int i = 0; i < medias.Count(); i++)
            {
                if (File.Exists(Path.Combine(hostingEnvironment.WebRootPath, "uploads", medias[i].ImagePath)))
                {
                    medias[i].ImagePath = DateTime.Now.ToString("dd-MM-yyyy hh.mm.ss.fff") + "_" + medias[i].ImagePath;
                }
            }

            return medias;
        }

        public User GetUserId(string LoginUser) //получаем id пользователя
        {
            var user = _context.Users.Where(u => u.Login == LoginUser).FirstOrDefault();
            return user;
        }
    }
}
