namespace CorporateDb.Shared.Dto;

public class UserProfileDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Otchestvo { get; set; }
    public string Email { get; set; }
    public string Section { get; set; }
    public DateTime? BirthDay { get; set; }
    public bool HasPrivate { get; set; }
}