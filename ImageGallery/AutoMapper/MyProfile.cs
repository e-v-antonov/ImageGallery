using AutoMapper;
using ImageGallery.Models;
using ImageGallery.ViewModels.AccountModel;
using ImageGallery.ViewModels.CategoryViewModels;
using ImageGallery.ViewModels.MediaViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Automapper
{
    public class MyProfile : Profile
    {
        public MyProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            CreateMap<Category, EditCategoryViewModel>().ReverseMap();
            CreateMap<Category, CreateCategoryViewModel>().ReverseMap();

            CreateMap<Media, MediaEditViewModel>().ReverseMap();
            CreateMap<Media, MediaViewModel>().ReverseMap();

            CreateMap<User, UserCreateViewModel>().ReverseMap();
            CreateMap<User, UserEditViewModel>().ReverseMap();
            CreateMap<User, UserViewModel>().ReverseMap();
            CreateMap<User, LoginViewModel>().ReverseMap();
        }
    }
}
