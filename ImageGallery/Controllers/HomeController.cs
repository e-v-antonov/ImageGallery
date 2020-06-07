using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ImageGallery.Models;
using ImageGallery.Infrastructure;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using ImageGallery.Data;
using Microsoft.EntityFrameworkCore;
using System;
using ImageGallery.ViewModels;
using System.IO;
using Ionic.Zip;
using Microsoft.AspNetCore.Authorization;

namespace ImageGallery.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly MyContext _context;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostingEnvironment, MyContext context)
        {
            _unitOfWork = unitOfWork;
            this.hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int pageNumber = 1)
        {
            ViewData["HomeCurrentSort"] = sortOrder;
            ViewData["TitleAlbumSortParm"] = String.IsNullOrEmpty(sortOrder) ? "TitleAlbumDESC" : "";
            ViewData["DateCreateAlbumHomeSortParm"] = sortOrder == "DateCreateAlbumASC" ? "DateCreateAlbumDESC" : "DateCreateAlbumASC";
            ViewData["CountImageSortParm"] = sortOrder == "CountImageASC" ? "CountImageDESC" : "CountImageASC";

            if (searchString != null)
            {
                pageNumber = 1;
            }                
            else
            {
                searchString = currentFilter;
            }                

            ViewData["HomeCurrentFilter"] = searchString;

            //выбор данных из БД
            var model = _unitOfWork.MediaRepo.GetAll(HttpContext.User.Identity.Name, HttpContext.User.IsInRole("Администратор"));

            //фильтрация данных
            if (!String.IsNullOrEmpty(searchString))
            {
                model = model.Where(m => m.Title.Contains(searchString));
            }

            //сортировка данных
            switch (sortOrder)
            {
                case "TitleAlbumDESC":
                    model = model.OrderByDescending(m => m.Title);
                    break;
                case "DateCreateAlbumASC":
                    model = model.OrderBy(m => m.TimeCreateAlbum);
                    break;
                case "DateCreateAlbumDESC":
                    model = model.OrderByDescending(m => m.TimeCreateAlbum);
                    break;
                case "CountImageASC":
                    model = model.OrderBy(m => m.CountImage);
                    break;
                case "CountImageDESC":
                    model = model.OrderByDescending(m => m.CountImage);
                    break;
                default:
                    model = model.OrderBy(m => m.Title);
                    break;
            }

            int pageSize = 10;

            //вывод в таблице только 10 записей
            var count = model.Count();
            var items = model.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList paginatedList = new PaginatedList(count, pageNumber, pageSize);
            HomeIndexViewModel homeIndexViewModel = new HomeIndexViewModel
            {
                PaginatedList = paginatedList,
                HomeModel = items
            };

            return View(homeIndexViewModel);
        }

        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult DisplayAlbum(int idAlbum, string nameAlbum) //открытие страницы с просмотром альбома
        {
            var modelDisplayImage = _context.Media.Include(x => x.Category).Where(x => x.CategoryId == idAlbum).ToList();

            ViewData["NameAlbum"] = nameAlbum;
            ViewData["IdAlbum"] = idAlbum;
            ViewData["DescriptionAlbum"] = _context.Categories.Where(c => c.Id == idAlbum).FirstOrDefault().DescriptionAlbum;

            return View(modelDisplayImage);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult DownloadImageFile(int idAlbum)  //скаичивание архива изображений
        {
            var files = _context.Media.Where(f => f.CategoryId == idAlbum).ToList();

            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                zip.AddDirectoryByName("Files");

                //добавление файлов в архив
                foreach (var item in files)
                {
                    zip.AddFile(Path.Combine(hostingEnvironment.WebRootPath, "uploads", item.ImagePath), "Files");
                }

                string zipName = String.Format("Zip_{0}.zip", DateTime.Now.ToString("dd-MM-yyyy-HH:mm:ss"));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    zip.Save(memoryStream);
                    return File(memoryStream.ToArray(), "application/zip", zipName);
                }
            }
        }
        
        [HttpPost]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public RedirectToRouteResult AddNewImage()  //загрузка нового изображения
        {
            return RedirectToRoute(new { controller= "modelController", action="Create" });
        }
    }
}
