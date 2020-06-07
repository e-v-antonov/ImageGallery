using System.Collections.Generic;
using System.Linq;
using ImageGallery.Data;
using ImageGallery.ViewModels.Charts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Controllers
{
    public class ChartController : Controller
    {
        private readonly MyContext _context;

        public ChartController(MyContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public IActionResult ChartIndex()
        {
            var chartModelOne = new List<CountUserAlbums>();
            var chartModelTwo = new List<CountAlbumByDate>();
            var chartModelThree = new List<CountMediaUser>();

            //выбор данных из базы данных
            chartModelOne = _context.Users.Include(u => u.Category).Select(u => new CountUserAlbums { Login = u.Login, CountAlbums = u.Category.Count() }).ToList();
            chartModelTwo = _context.Categories.GroupBy(c => c.TimeCreateAlbum).Select(g => new CountAlbumByDate { TimeCreateAlbum = g.Key, CountAlbums = g.Count() }).ToList();
            chartModelThree = _context.Media.Include(m => m.Category.User).GroupBy(m => m.Category.User.Login).Select(g => new CountMediaUser { Login = g.Key, CountMedia = g.Count()}).ToList();

            //конвертация данных
            var XLabelsOne = Newtonsoft.Json.JsonConvert.SerializeObject(chartModelOne.Select(x => x.Login).ToList());
            var YValuesOne = Newtonsoft.Json.JsonConvert.SerializeObject(chartModelOne.Select(y => y.CountAlbums).ToList());

            var XLabelsTwo = Newtonsoft.Json.JsonConvert.SerializeObject(chartModelTwo.Select(x => x.TimeCreateAlbum).ToList());
            var YValuesTwo = Newtonsoft.Json.JsonConvert.SerializeObject(chartModelTwo.Select(y => y.CountAlbums).ToList());

            var XLabelsThree = Newtonsoft.Json.JsonConvert.SerializeObject(chartModelThree.Select(y => y.Login).ToList());
            var YValuesThree = Newtonsoft.Json.JsonConvert.SerializeObject(chartModelThree.Select(y => y.CountMedia).ToList());

            ViewBag.XLabelsOne = XLabelsOne;
            ViewBag.YValuesOne = YValuesOne;

            ViewBag.XLabelsTwo = XLabelsTwo;
            ViewBag.YValuesTwo = YValuesTwo;

            ViewBag.XLabelsThree = XLabelsThree;
            ViewBag.YValuesThree = YValuesThree;

            return View();
        }
    }
}
