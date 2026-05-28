using CidadeMelhorApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace CidadeMelhorApi.Controllers
{
    [Route("api/Admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(AdminLogin login)
        {
            var admin = _context.Admins.FirstOrDefault(x =>
                x.Email == login.Email &&
                x.Senha == login.Senha);

            if (admin == null)
                return Unauthorized("Email ou senha do admin invalidos.");

            return Ok(new
            {
                admin.Id,
                admin.Nome,
                admin.Email
            });
        }

        public class AdminLogin
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }
    }
}
