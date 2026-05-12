using CidadeMelhorApi.Data;
using Microsoft.AspNetCore.Mvc;
using CidadeMelhorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CidadeMelhorApi.Controllers
{
    [Route("api/Denuncias")]
    [ApiController]
    
    public class DenunciaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DenunciaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Denuncias.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var denuncia = _context.Denuncias.FirstOrDefault(d => d.Id == id);
            if (denuncia == null)
            {
                return NotFound();
            }
            return Ok(denuncia);
        }
        
        [HttpPost]
        public IActionResult Create(Denuncia denuncia)
        {
            _context.Denuncias.Add(denuncia);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = denuncia.Id }, denuncia);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, Denuncia novaDenuncia)
        {
            var denuncia = _context.Denuncias.Find(id);

            if (denuncia == null)
                return NotFound();

            denuncia.Descricao = novaDenuncia.Descricao;
            denuncia.Categoria = novaDenuncia.Categoria;
            denuncia.Endereco = novaDenuncia.Endereco;
            denuncia.Status = novaDenuncia.Status;

            _context.SaveChanges();

            return Ok(denuncia);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var denuncia = _context.Denuncias.Find(id);

            if (denuncia == null)
                return NotFound();

            _context.Denuncias.Remove(denuncia);

            _context.SaveChanges();

            return Ok("Denúncia removida");
        }

        [HttpGet("filter")]
        public IActionResult Filter(
            string? categoria,
            string? status,
            string? Endereco
        )
        {
            var query = _context.Denuncias.AsQueryable();

            if (!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(x => x.Categoria == categoria);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrEmpty(Endereco))
            {
                query = query.Where(x =>
                    x.Endereco != null &&
                    x.Endereco.Contains(Endereco)
                );
            }

            return Ok(query.ToList());
        }
    }
}
