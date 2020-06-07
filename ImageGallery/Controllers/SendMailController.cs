using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImageGallery.AdditionalClass;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.Controllers
{
    public class SendMailController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public IActionResult SendMail()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Сотрудник")]
        public async Task<IActionResult> SendMessage(string email, string subject, string message)
        {
            string regex = @"^[^@]+@[^@.]+\.[^@]+$";

            try
            {
                if (Regex.IsMatch(email, regex))    //если emailвведен верно
                {
                    EmailService emailService = new EmailService();
                    await emailService.SendEmailAsync(email, subject, message);
                    return RedirectToAction("SendMail");
                }
                else
                {
                    TempData["errorEmail"] = "Неправильно указан Email";
                    return RedirectToAction("SendMail");
                }
            }
            catch
            {
                throw new System.Exception("Не удалось отправит сообщение. Повторите попытку.");
            }
        }
    }
}
