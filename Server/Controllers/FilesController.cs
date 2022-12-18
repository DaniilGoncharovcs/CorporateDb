using System.Net;
using System.Net.Http.Headers;
namespace CorporateDb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hosting;
    public FilesController(ApplicationDbContext context, IWebHostEnvironment hosting)
    {
        _context = context;
        _hosting = hosting;
    }


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
    //[Authorize(Roles = "admin")]
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

    [HttpPost("upload")]
    public IActionResult Upload()
    {
        try
        {
            var file = Request.Form.Files[0];
            var folderName = Path.Combine("Files");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Ok(dbPath);
            }
            else
            {
                return BadRequest();
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex}");
        }
    }

    [HttpGet("Download/{filename}")]
    public async Task<IActionResult> Download(string fileName)
    {
        if (string.IsNullOrWhiteSpace(_hosting.WebRootPath))
        {
            _hosting.WebRootPath = Path.Combine(_hosting.ContentRootPath, "Files");
        }
        string myFilePath = _hosting.WebRootPath + $@"\{fileName}";
        byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(myFilePath);
        return File(fileBytes, "application/octet-stream", fileName);

    }
}