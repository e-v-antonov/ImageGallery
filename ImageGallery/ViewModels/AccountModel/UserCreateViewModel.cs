using System.ComponentModel.DataAnnotations;

namespace ImageGallery.ViewModels.AccountModel
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Пожалуйста, укажите фамилию пользователя.")]
        [StringLength(30, ErrorMessage = "Длина фамилии должна быть от 1 до 30 символов.", MinimumLength = 1)]
        [RegularExpression(@"^[а-яА-Я].\S+$", ErrorMessage = "Фамилия должна состоять только из русских букв.")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите имя пользователя.")]
        [StringLength(20, ErrorMessage = "Длина имени должна быть от 1 до 20 символов.", MinimumLength = 1)]
        [RegularExpression(@"^[а-яА-Я].\S+$", ErrorMessage = "Имя должно состоять только из русских букв.")]
        public string Name { get; set; }

        [StringLength(30, ErrorMessage = "Отчество должно быть длиной до 30 символов.", MinimumLength = 0)]
        [RegularExpression(@"^[а-яА-Я].\S+$", ErrorMessage = "Отчество должно состоять только из русских букв.")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите логин пользователя.")]
        [StringLength(16, ErrorMessage = "Длина логина должна быть от 8 до 16 символов.", MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-Z0-9].\S+$", ErrorMessage = "Логин должен состоять только из английских букв и цифр.")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пожалуйста, укажите пароль пользователя.")]
        [StringLength(16, ErrorMessage = "Длина пароля должна быть от 8 до 16 символов.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).\S+$", ErrorMessage = "Пароль должен обязательно содержать хотя бы одну строчную и заглавную английскую букву, цифру и специальный символ и состоять только из английских букв")]
        public string Password { get; set; }

        public int RoleId { get; set; }
    }
}
