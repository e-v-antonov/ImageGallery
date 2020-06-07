using ImageGallery.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.Infrastructure
{
    public interface IUnitOfWork
    {
        ICategoryRepo CategoryRepo { get; }

        IMediaRepo MediaRepo { get; }

        void Save();

        void UploadFile(List<IFormFile> files, List<Media> medias);

        List<Media> CheckFileName(List<Media> medias);

        User GetUserId(string LoginUser);
    }
}
