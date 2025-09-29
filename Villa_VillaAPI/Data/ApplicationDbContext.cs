using Microsoft.EntityFrameworkCore;
using Villa_VillaAPI.Model.Entity;

namespace Villa_VillaAPI.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        //{}
        public DbSet<Villa> Villas { get; set; }
    }
}
