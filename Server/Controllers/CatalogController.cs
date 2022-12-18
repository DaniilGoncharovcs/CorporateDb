namespace CorporateDb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CatalogController(ApplicationDbContext context)
        => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetCatalogs()
    {
        var catalogList = await _context.Catalogs.ToListAsync();

        return Ok(catalogList);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCatalogDetails(int id)
    {
        var catalog = await _context.Catalogs
            .Include(c => c.Files)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (catalog == null)
            return BadRequest("Каталога с таким Id не существует");

        var files = new List<FileModelDto>();

        if(catalog.Files.Count > 0)
        {
            foreach(var file in catalog.Files)
            {
                var fileDto = new FileModelDto
                {
                    Id = file.Id,
                    Name = file.Name,
                    Format = file.Format,
                    Size = file.Size,
                    IsPublic = file.IsPublic,
                };
                files.Add(fileDto);
            }
        }

        var result = new GetCatalogDetails
        {
            Id = catalog.Id,
            Name = catalog.Name,
            Files = files
        };

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateCatalog(CreateCatalogDto catalogDto)
    {
        var catalog = new Catalog
        {
            Name = catalogDto.Name,
        };

        await _context.Catalogs.AddAsync(catalog);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteCatalog(int id)
    {
        var catalog = await _context.Catalogs.FirstOrDefaultAsync(
            c => c.Id == id);

        if (catalog == null)
            return BadRequest($"Каталога с таким id:{id} не существует");

        _context.Remove(catalog);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}