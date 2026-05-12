using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CidadeMelhorApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CidadeMelhorApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<Denuncia> Denuncias {get; set;}
        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Admin> Admins {get; set;}
    }
}