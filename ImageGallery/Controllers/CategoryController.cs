using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImageGallery.Infrastructure;
using ImageGallery.Models;
using ImageGallery.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int pageNumber = 1)
        {
            ViewData["CategoryCurrentSort"] = sortOrder;
            ViewData["TitleCategorySortParm"] = String.IsNullOrEmpty(sortOrder) ? "TitleCategoryDESC" : "";
            ViewData["TimeCreateAlbumSortParm"] = sortOrder == "TimeCreateAlbumASC" ? "TimeCreateAlbumDESC" : "TimeCreateAlbumASC";

            if (searchString != null)
            {
                pageNumber = 1;
            }                
            else
            {
                searchString = currentFilter;
            }                

            ViewData["CategoryCurrentFilter"] = searchString;

            var category = _unitOfWork.CategoryRepo.GetAll(HttpContext.User.Identity.Name, HttpContext.User.IsInRole("Администратор"));

            //фильтрация данных
            if (!String.IsNullOrEmpty(searchString))
            {
                category = category.Where(c => c.Title.Contains(searchString));
            }
            
            //сортировка данных
            switch (sortOrder)
            {
                case "TitleCategoryDESC":
                    category = category.OrderByDescending(c => c.Title);
                    break;
                case "TimeCreateAlbumASC":
                    category = category.OrderBy(c => c.TimeCreateAlbum);
                    break;
                case "TimeCreateAlbumDESC":
                    category = category.OrderByDescending(c => c.TimeCreateAlbum);
                    break;
                default:
                    category = category.OrderBy(c => c.Title);
                    break;
            }

            var vm = _mapper.Map<List<CategoryViewModel>>(category);

            int pageSize = 10;

            //вывод записей по 10 штук в таблице
            var count = vm.Count();
            var items = vm.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList paginatedList = new PaginatedList(count, pageNumber, pageSize);
            CategoryIndexViewModel categoryIndexViewModel = new CategoryIndexViewModel
            {
                PaginatedList = paginatedList,
                Categories = items
            };

            return View(categoryIndexViewModel);            
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Create(CreateCategoryViewModel category)
        {
            string userLogin = HttpContext.User.Identity.Name;
            int id = _unitOfWork.GetUserId(userLogin).Id;

            category.UserId = id;

            var mappedCategory = _mapper.Map<Category>(category);
            _unitOfWork.CategoryRepo.Insert(mappedCategory);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = _unitOfWork.CategoryRepo.GetById(id);
            var mappedCategory = _mapper.Map<EditCategoryViewModel>(category);

            return View(mappedCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Edit(EditCategoryViewModel vm)
        {
            var category = _mapper.Map<Category>(vm);

            _unitOfWork.CategoryRepo.Update(category);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public ActionResult Delete(int id)
        {
            var category = _unitOfWork.CategoryRepo.GetById(id);
            var mappedCategory = _mapper.Map<CategoryViewModel>(category);

            return View(mappedCategory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int Id, CategoryViewModel category)
        {
            _unitOfWork.CategoryRepo.Delete(Id);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}