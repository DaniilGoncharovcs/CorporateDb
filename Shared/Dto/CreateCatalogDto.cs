namespace CorporateDb.Shared.Dto;

public class CreateCatalogDto
{
    [Required(ErrorMessage = "Название каталога обязательно для заполнения")]
    [MinLength(5, ErrorMessage = "Минимальное количество символов для названия каталога 5")]
    [MaxLength(20, ErrorMessage = "Максимальное количество символов для названия каталога 20")]
    [RegularExpression("[А-я,A-z,0-9, ,\",№,:,(,),-,_,.]{1,}")]
    public string Name { get; set; }
}