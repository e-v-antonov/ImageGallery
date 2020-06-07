using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using ImageGallery.Data;
using ImageGallery.Infrastructure;
using ImageGallery.Models;
using ImageGallery.ViewModels.MediaViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Controllers
{
    public class MediaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MyContext _context;
        private readonly IWebHostEnvironment hostingEnvironment;

        public MediaController(IUnitOfWork unitOfWork, IMapper mapper, MyContext context, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int pageNumber = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ImagePathSortParm"] = String.IsNullOrEmpty(sortOrder) ? "ImagePathDESC" : "";
            ViewData["CategorySortParm"] = sortOrder == "CategoryASC" ? "CategoryDESC" : "CategoryASC";

            if (searchString != null)
            {
                pageNumber = 1;
            }                
            else
            {
                searchString = currentFilter;
            }                

            ViewData["CurrentFilter"] = searchString;

            //выборка данных из базы данных
            var media = _unitOfWork.MediaRepo.GetAllMedia(HttpContext.User.Identity.Name, HttpContext.User.IsInRole("Администратор"));

            //фильтрация данных
            if (!String.IsNullOrEmpty(searchString))
            {
                media = media.Where(m => m.ImagePath.Contains(searchString) || m.Category.Title.Contains(searchString));
            }

            //сортировка данных
            switch (sortOrder)
            {
                case "ImagePathDESC":
                    media = media.OrderByDescending(m => m.ImagePath);
                    break;
                case "CategoryASC":
                    media = media.OrderBy(m => m.CategoryId);
                    break;
                case "CategoryDESC":
                    media = media.OrderByDescending(m => m.CategoryId);
                    break;
                default:
                    media = media.OrderBy(m => m.ImagePath);
                    break;
            }

            var vm = _mapper.Map<List<MediaViewModel>>(media);

            int pageSize = 10;
                
            //вывод только 10 записей в таблице
            var count = vm.Count();
            var items = vm.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList paginatedList = new PaginatedList(count, pageNumber, pageSize);
            MediaIndexViewModel mediaIndexViewModel = new MediaIndexViewModel
            {
                PaginatedList = paginatedList,
                Medias = items
            };

            return View(mediaIndexViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Create()
        {
            ViewBag.Categories = _unitOfWork.CategoryRepo.GetAll(HttpContext.User.Identity.Name, HttpContext.User.IsInRole("Администратор"));

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Create(MediaCreateViewModel vm)     //добавление изображения
        {
            try
            {
                var category = _unitOfWork.CategoryRepo.GetById(vm.CategoryId);

                List<Media> media = new List<Media>();

                foreach (var file in vm.Files)
                {
                    media.Add(new Media()
                    {
                        ImagePath = file.FileName,
                        Category = category
                    });
                }

                foreach (var item in media)
                {
                    string fileExtension = item.ImagePath.Substring(item.ImagePath.LastIndexOf('.'));

                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".gif" &&
                        fileExtension != ".bmp" && fileExtension != ".ico")
                        throw new Exception("Данный формат не поддерживается для загрузки.");
                }

                media = _unitOfWork.CheckFileName(media);
                _unitOfWork.UploadFile(vm.Files, media);
                _unitOfWork.MediaRepo.AddRange(media);
                _unitOfWork.Save();

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Categories = _unitOfWork.CategoryRepo.GetAll(HttpContext.User.Identity.Name, HttpContext.User.IsInRole("Администратор"));
                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Edit(int id)
        {
            ViewBag.Categories = _unitOfWork.CategoryRepo.GetAll(HttpContext.User.Identity.Name, HttpContext.User.IsInRole("Администратор"));

            var media = _unitOfWork.MediaRepo.GetById(id);
            var mappedMedia = _mapper.Map<MediaEditViewModel>(media);

            return View(mappedMedia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Edit(MediaEditViewModel vm) //изменение изображения
        {
            try
            {
                if (vm.File == null && _unitOfWork.MediaRepo.GetById(vm.Id).CategoryId == vm.CategoryId)    //если файл не выбран
                {
                    RedirectToAction(nameof(Index));
                }
                else
                {
                    if (vm.File != null)    //если файл выбран
                    {
                        List<IFormFile> files = new List<IFormFile>();
                        files.Add(vm.File);

                        var updateMedia = _unitOfWork.MediaRepo.GetById(vm.Id);

                        RemoveFile(updateMedia.ImagePath);  //удаляем строе изображение

                        updateMedia.CategoryId = vm.CategoryId;
                        updateMedia.ImagePath = vm.File.FileName;
                        List<Media> media = new List<Media>();
                        media.Add(updateMedia); //заносим данные нового изображения в базу данных
                        
                        _unitOfWork.UploadFile(files, media);   //загружаем новое изображение
                        _unitOfWork.MediaRepo.Update(updateMedia);
                        _unitOfWork.Save();

                        RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        if (_unitOfWork.MediaRepo.GetById(vm.Id).CategoryId != vm.CategoryId)   //если изображение помещается в другой альбом
                        {
                            List<IFormFile> files = new List<IFormFile>();
                            files.Add(vm.File);

                            var updateMedia = _unitOfWork.MediaRepo.GetById(vm.Id);
                            updateMedia.CategoryId = vm.CategoryId;
                            updateMedia.ImagePath = _unitOfWork.MediaRepo.GetById(vm.Id).ImagePath;

                            _unitOfWork.MediaRepo.Update(updateMedia);
                            _unitOfWork.Save();

                            RedirectToAction(nameof(Index));
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Delete(int id)
        {
            var model = _unitOfWork.MediaRepo.GetById(id);
            var vm = _mapper.Map<MediaViewModel>(model);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Delete(int Id, IFormCollection collection)  //удаление изображения
        {
            try
            {
                var fileModel = _context.Media.SingleOrDefaultAsync(m => m.Id == Id);
                RemoveFile(fileModel.Result.ImagePath);

                _unitOfWork.MediaRepo.Delete(Id);
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public PhysicalFileResult DownloadImage(int id) //скачивание изображения
        {
            var model = _unitOfWork.MediaRepo.GetById(id);

            string pathFile = Path.Combine(hostingEnvironment.WebRootPath, "uploads", model.ImagePath);
            string fileType = "image/" + model.ImagePath.Substring(model.ImagePath.LastIndexOf('.') + 1); 

            return PhysicalFile(pathFile, fileType, model.ImagePath);
        }

        private bool RemoveFile(string path)    //удаление самого изображения с сервера
        {
            string fullPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads", path);

            if (!System.IO.File.Exists(fullPath))
            {
                return false;
            }                

            try
            {
                System.IO.File.Delete(fullPath);
                return true;
            }
            catch
            {
                throw new Exception("Не удалось удалить файлы изображения из системы.");
            }            
        }
    }
}