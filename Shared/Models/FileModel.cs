namespace CorporateDb.Shared.Models;

public class FileModel
{
    public int Id { get; set; }
    [MinLength(7, ErrorMessage = "Минимальная длина для названия документа 7 символов")]
    [MaxLength(20, ErrorMessage = "Максимальная длина для названия документа 20 символов")]
    [Required(ErrorMessage = "Название файла обязательно для заполненеия")]
    [RegularExpression("^[А-я,A-z,0-9, ,\",№,:,(,),-,_,.]")]
    public string Name { get; set; }
    [Required]    
    public Formats Format { get; set; }
    [Required]
    [Range(0, 20971521, ErrorMessage = "Максимальная размер файла 20 МВ")]
    public int Size { get; set; }
    public bool IsPublic { get; set; }
    public int CatalogId { get; set; }
    public Catalog Catalog { get; set; }
}