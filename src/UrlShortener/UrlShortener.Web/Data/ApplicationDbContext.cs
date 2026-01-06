using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Entities.UrlShortener> UrlShorteners { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
