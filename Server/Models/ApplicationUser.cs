using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CorporateDb.Server.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Имя обязательно для заполнения")]
    public string LastName { get; set; }
    [Required(ErrorMessage = "Отчество обязательно для заполнения")]
    public string Otchestvo { get; set; }
    public DateTime? BirthDay { get; set; }
    public string? Section { get; set; }
    public string? RefreshToken { get; set; }
    public bool? ReadPrivate { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}