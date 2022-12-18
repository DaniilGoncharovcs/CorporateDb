using CorporateDb.Server.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CorporateDb.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public DbSet<Catalog> Catalogs { get; set; }
    public DbSet<FileModel> Files { get; set; }
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {   

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var ADMIN_ID = Guid.NewGuid().ToString();
        var ADMINROLE_ID = Guid.NewGuid().ToString();

        builder.Entity<IdentityRole>()
            .HasData(new IdentityRole
            {
                Name = "admin",
                NormalizedName = "ADMIN",
                Id = ADMINROLE_ID,
                ConcurrencyStamp = ADMINROLE_ID
            },
            new IdentityRole
            {
                Name = "employee",
                NormalizedName = "EMPLOYEE"
            },
            new IdentityRole
            {
                Name = "private",
                NormalizedName = "PRIVATE"
            });

        var admin = new ApplicationUser
        {
            Id = ADMIN_ID,
            Email = "ivanov@mail.ru",
            FirstName = "Иванов",
            LastName = "Иван",
            Otchestvo = "Иванович",
            UserName = "ivanov@mail.ru",
            BirthDay = new DateTime(2000, 1, 1),
            NormalizedEmail = "IVANOV@MAIL.RU",
            NormalizedUserName = "IVANOV@MAIL.RU",
            ReadPrivate = true,
        };

        var hasher = new PasswordHasher<ApplicationUser>();
        admin.PasswordHash = hasher.HashPassword(admin, "4glzvOEhxR");

        builder.Entity<ApplicationUser>()
            .HasData(admin);

        builder.Entity<IdentityUserRole<string>>()
            .HasData(new IdentityUserRole<string>
            {
                RoleId = ADMINROLE_ID,
                UserId = ADMIN_ID
            });
    }
}