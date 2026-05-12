using Microsoft.AspNetCore.Mvc;

namespace CidadeMelhorApi.Controllers
{
    [Route("api/Admins")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private const string EmailAdmin = "admin@gmail.com";
        private const string SenhaAdmin = "admin123";

        [HttpPost("login")]
        public IActionResult Login(AdminLogin login)
        {
            if (login.Email == EmailAdmin && login.Senha == SenhaAdmin)
            {
                return Ok(new
                {
                    id = 1,
                    nome = "Administrador",
                    email = EmailAdmin
                });
            }

            return Unauthorized("Email ou senha do admin invalidos.");
        }

        public class AdminLogin
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }
    }
}
