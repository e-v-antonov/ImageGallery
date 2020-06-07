using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using ImageGallery.AdditionalClass;
using ImageGallery.Data;
using ImageGallery.Infrastructure;
using ImageGallery.Models;
using ImageGallery.ViewModels.AccountModel;
using ImageGallery.ViewModels.CategoryViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyContext _context;
        private readonly IAccountRepo _accountRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountController(MyContext context, IAccountRepo accountRepo, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _context = context;
            _accountRepo = accountRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public IActionResult UserIndex(string sortOrder, string currentFilter, string searchString, int pageNumber = 1)
        {
            ViewData["UserCurrentSort"] = sortOrder;
            ViewData["SurnameUserSortParm"] = String.IsNullOrEmpty(sortOrder) ? "SurnameUserDESC" : "";
            ViewData["NameUserSortParm"] = sortOrder == "NameUserASC" ? "NameUserDESC" : "NameUserASC";
            ViewData["PatronymicUserSortParm"] = sortOrder == "PatronymicUserASC" ? "PatronymicUserDESC" : "PatronymicUserASC";
            ViewData["LoginUserSortParm"] = sortOrder == "LoginUserASC" ? "LoginUserDESC" : "LoginUserASC";
            ViewData["RoleUserSortParm"] = sortOrder == "RoleUserASC" ? "RoleDESC" : "RoleASC";

            if (searchString != null)
            {
                pageNumber = 1;
            }                
            else
            {
                searchString = currentFilter;
            }                

            ViewData["UserCurrentFilter"] = searchString;

            var user = _accountRepo.GetAll();

            //фильтрация записей
            if (!String.IsNullOrEmpty(searchString))
            {
                user = user.Where(u => u.Surname.Contains(searchString) || u.Name.Contains(searchString) || u.Patronymic.Contains(searchString) ||
                    u.Login.Contains(searchString) || u.Role.NameRole.Contains(searchString));
            }

            //сортировка записей
            switch (sortOrder)
            {
                case "SurnameUserDESC":
                    user = user.OrderByDescending(u => u.Surname);
                    break;
                case "NameUserASC":
                    user = user.OrderBy(u => u.Name);
                    break;
                case "NameUserDESC":
                    user = user.OrderByDescending(u => u.Name);
                    break;
                case "PatronymicUserASC":
                    user = user.OrderBy(u => u.Patronymic);
                    break;
                case "PatronymicUserDESC":
                    user = user.OrderByDescending(u => u.Patronymic);
                    break;
                case "LoginUserASC":
                    user = user.OrderBy(u => u.Login);
                    break;
                case "LoginUserDESC":
                    user = user.OrderByDescending(u => u.Login);
                    break;
                case "RoleUserASC":
                    user = user.OrderBy(u => u.Role.NameRole);
                    break;
                case "RoleDESC":
                    user = user.OrderByDescending(u => u.Role.NameRole);
                    break;
                default:
                    user = user.OrderBy(u => u.Surname);
                    break;
            }

            int pageSize = 10;

            var vm = _mapper.Map<List<UserViewModel>>(user);

            //вывод на страницу только 10 записей
            var count = vm.Count();
            var items = vm.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            PaginatedList paginatedList = new PaginatedList(count, pageNumber, pageSize);
            UserIndexViewModel userIndexViewModel = new UserIndexViewModel
            {
                PaginatedList = paginatedList,
                Users = items
            };

            return View(userIndexViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = _context.Roles.OrderByDescending(r => r.Id);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public IActionResult CreateUser(UserCreateViewModel model)
        {
            //string regex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).\S+$";

            //if (Regex.IsMatch(model.Password, regex))    //если emailвведен верно
            //{
            //    var i = 0;
            //}


                if (ModelState.IsValid)
            {
                //шифрование пароля
                CryptoPassword cryptoPassword = new CryptoPassword();
                model.Password = cryptoPassword.Encrypt(model.Password);

                var mappedUser = _mapper.Map<User>(model);

                //добавление данных в БД
                _accountRepo.Insert(mappedUser);
                _unitOfWork.Save();

                ViewBag.Roles = _context.Roles.OrderByDescending(r => r.Id);
                return RedirectToAction("UserIndex", "Account");
            }

            ViewBag.Roles = _context.Roles;
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public ActionResult EditUser(int id)
        {
            ViewBag.Roles = _context.Roles;

            var user = _accountRepo.GetById(id);

            //расшифровка пароля
            CryptoPassword cryptoPassword = new CryptoPassword();
            user.Password = cryptoPassword.Decrypt(user.Password);

            var mappedUser = _mapper.Map<UserEditViewModel>(user);

            return View(mappedUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public ActionResult EditUser(UserEditViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                //шифрование пароля
                CryptoPassword cryptoPassword = new CryptoPassword();
                viewmodel.Password = cryptoPassword.Encrypt(viewmodel.Password);

                var user = _mapper.Map<User>(viewmodel);

                _accountRepo.Update(user, HttpContext.User.Identity.Name);
                _unitOfWork.Save();

                return RedirectToAction(nameof(UserIndex));
            }
            else
            {
                ViewBag.Roles = _context.Roles;
                return View(viewmodel);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public ActionResult DeleteUser(int id)
        {
            var user = _accountRepo.GetById(id);
            var mappedUser = _mapper.Map<UserViewModel>(user);

            return View(mappedUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Администратор")]
        public ActionResult DeleteUser(int Id, CategoryViewModel category)
        {
            _accountRepo.Delete(Id, HttpContext.User.Identity.Name);
            _unitOfWork.Save();

            return RedirectToAction(nameof(UserIndex));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                //шифрование пароля
                CryptoPassword cryptoPassword = new CryptoPassword();
                model.Password = cryptoPassword.Encrypt(model.Password);

                //нахождение пользователя в БД
                User user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    TempData["PkUser"] = user.Id;
                    TempData["RoleUser"] = user.RoleId;                    

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IActionResult> Logout()   //выход из аккаунта
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task Authenticate(User user)  //аутентификация
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.NameRole)
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);

            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
