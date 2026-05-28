using CidadeMelhorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CidadeMelhorApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Denuncia> Denuncias { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Admin> Admins { get; set; }
    }
}
