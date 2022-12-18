namespace CorporateDb.Shared.Dto;

public class UserForRegistrationDto
{
    [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Отчество обязательно для заполнения")]
    public string Otchestvo { get; set; }
    public DateTime BirthDay { get; set; }
    [Required(ErrorMessage = "Отдел обязателен для заполнения")]
    public string Section { get; set; }
    [Required(ErrorMessage = "Уровень доступа обязателен для ввода")]
    public bool ReadPrivate { get; set; }
    [Required(ErrorMessage = "Пароль обязателен для ввода")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    [Required(ErrorMessage = "Email обязателен")]
    [DataType(DataType.EmailAddress, ErrorMessage = "Неккорректный формат email")]
    public string Email { get; set; }
}