namespace CorporateDb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FilesController(ApplicationDbContext context)
        => _context = context;


    [HttpGet("{catalogid}/privatefiles")]
    [Authorize(Roles = "admin,private")]
    public async Task<IActionResult> GetFiles(int catalogid)
    {
        var files = await _context.Files.Where(f => f.CatalogId == catalogid).ToListAsync();

        return Ok(files);
    }

    [HttpGet("{catalogid}/publicfiles")]
    public async Task<IActionResult> GetPublicFiles(int catalogid)
    {
        var files = await _context.Files.Where(f => f.CatalogId == catalogid && f.IsPublic == true).ToListAsync();

        return Ok(files);
    }


    [HttpPost("{catalogid}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AddNewFile(CreateFileModelDto fileModelDto, int catalogid)
    {
        if(!ModelState.IsValid)
        {
            var errors = ModelState.Select(x => x.Value.Errors)
                .Where(y => y.Count > 0)
                .ToList();

            return BadRequest(errors);
        }

        var catalog = await _context.Catalogs.FirstOrDefaultAsync(c => c.Id == catalogid);
        if (catalog == null)
            return BadRequest($"Каталога с таким id:{catalogid} не существует");

        var fileModel = new FileModel
        {
            Name = fileModelDto.Name,
            Size = fileModelDto.Size,
            Format = fileModelDto.Format,
            CatalogId = catalogid
        };

        await _context.Files.AddAsync(fileModel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("id")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);

        if (file == null)
            return BadRequest($"Файла с таким id:{id} не существует");

        _context.Files.Remove(file);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null)
            return BadRequest("File is required");

        var fileName = file.FileName;

        var extension = Path.GetExtension(fileName);

        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Files");
        var fullPath = Path.Combine(directoryPath, fileName);

        Directory.CreateDirectory(directoryPath);
        using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await file.CopyToAsync(fileStream);
        }
        return Ok();
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var fileModel = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);

        if (fileModel == null)
            return BadRequest($"Файла с таким id:{id} не существует");

        var path = Path.Combine(Directory.GetCurrentDirectory(), "Files", fileModel.Name);

        if(System.IO.File.Exists(path))
        {
            return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
        }

        return BadRequest("Физического файла нет");
    }
}