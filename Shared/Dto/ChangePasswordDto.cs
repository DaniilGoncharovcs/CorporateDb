namespace CorporateDb.Shared.Dto;

public class ChangePasswordDto
{
    public string Id { get; set; }
    [Required(ErrorMessage = "Старый пароль обязателен для ввода")]
    public string OldPassword { get; set; }
    [Required(ErrorMessage = "Пароль обязателен для ввода")]
    public string Password { get; set; }
    [Compare("Password", ErrorMessage = "Введенные пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}