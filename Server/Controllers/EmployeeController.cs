namespace CorporateDb.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EmployeeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        => (_context,_userManager) = (context,userManager);

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var users = await _context.Users.ToListAsync();

        var usersList = new List<UserProfileDto>();

        foreach(var user in users)
        {
            var profile = new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Otchestvo = user.Otchestvo,
                Email = user.Email,
                Section = user.Section,
                BirthDay = user.BirthDay
            };
            usersList.Add(profile);
        }

        return Ok(usersList);
    }

    [HttpGet("{email}")]
    public async Task<IActionResult> GetProfileByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.UserName == email);

        if (user == null)
            return BadRequest($"Пользователя с таким email:{email} не существует");

        var userProfile = new UserProfileDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Otchestvo = user.Otchestvo,
            BirthDay = user.BirthDay,
            Section = user.Section,
            Email = user.Email,
        };

        return Ok(userProfile);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(UserForRegistrationDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user != null)
            return BadRequest();

        var appUser = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Otchestvo = dto.Otchestvo,
            Email = dto.Email,
            Section = dto.Section,
            BirthDay = dto.BirthDay,
            ReadPrivate = dto.ReadPrivate,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(appUser, dto.Password);
        if(result.Succeeded)
        {
            var userFromDb = await _userManager.FindByEmailAsync(dto.Email);

            await _userManager.AddToRoleAsync(userFromDb, "employee");

            if (userFromDb.ReadPrivate == true)
                await _userManager.AddToRoleAsync(userFromDb, "private");
            
            return NoContent();
        }

        return StatusCode(500);
    }
}