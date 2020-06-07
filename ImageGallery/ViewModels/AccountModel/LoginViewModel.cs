using System.ComponentModel.DataAnnotations;

namespace ImageGallery.ViewModels.AccountModel
{
    public class LoginViewModel
    {
        [StringLength(16, ErrorMessage = "Длина логина должна быть от 8 до 16 символов.", MinimumLength = 8)]
        public string Login { get; set; }

        [StringLength(16, ErrorMessage = "Длина пароля должна быть от 8 до 16 символов.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
