using CidadeMelhorApi.Data;
using CidadeMelhorApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CidadeMelhorApi.Controllers
{
    [Route("api/Usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var usuarios = _context.Usuarios
                .ToList()
                .Select(MontarResposta);

            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            return Ok(MontarResposta(usuario));
        }

        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nome) ||
                string.IsNullOrWhiteSpace(usuario.Email) ||
                string.IsNullOrWhiteSpace(usuario.Senha))
            {
                return BadRequest("Nome, email e senha sao obrigatorios.");
            }

            usuario.Cpf ??= string.Empty;

            var emailJaExiste = _context.Usuarios.Any(x => x.Email == usuario.Email);

            if (emailJaExiste)
                return BadRequest("Este email ja esta cadastrado.");

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = usuario.Id }, MontarResposta(usuario));
        }

        [HttpPost("login")]
        public IActionResult Login(UsuarioLogin login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) ||
                string.IsNullOrWhiteSpace(login.Senha))
            {
                return BadRequest("Email e senha sao obrigatorios.");
            }

            var usuario = _context.Usuarios.FirstOrDefault(x =>
                x.Email == login.Email &&
                x.Senha == login.Senha
            );

            if (usuario == null)
                return Unauthorized("Email ou senha invalidos.");

            return Ok(MontarResposta(usuario));
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePassword(int id, UsuarioSenha usuarioAtualizado)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            usuario.Senha = usuarioAtualizado.Senha;
            _context.SaveChanges();

            return Ok(MontarResposta(usuario));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
                return NotFound("Usuario nao encontrado.");

            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            return Ok("Usuario removido.");
        }

        public class UsuarioLogin
        {
            public string Email { get; set; } = string.Empty;
            public string Senha { get; set; } = string.Empty;
        }

        public class UsuarioSenha
        {
            public string Senha { get; set; } = string.Empty;
        }

        private static object MontarResposta(Usuario usuario)
        {
            return new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.Cpf
            };
        }
    }
}
