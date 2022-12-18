namespace CorporateDb.Shared.Dto;

public class GetCatalogDetails
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Название каталога обязательно для заполнения")]
    [MinLength(5, ErrorMessage = "Минимальное количество символов для названия каталога 5")]
    [MaxLength(20, ErrorMessage = "Максимальное количество символов для названия каталога 20")]
    [RegularExpression("^[А-я,A-z,0-9, ,\",№,:,(,),-,_,.]+")]
    public string Name { get; set; }
    public List<FileModelDto> Files { get; set; }
}