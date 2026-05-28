using CidadeMelhorApi.Data;
using Microsoft.AspNetCore.Mvc;
using CidadeMelhorApi.Models;

namespace CidadeMelhorApi.Controllers
{
    [Route("api/Denuncias")]
    [ApiController]
    public class DenunciaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DenunciaController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ListarDenuncias());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var denuncia = ListarDenuncias().FirstOrDefault(d => d.Id == id);

            if (denuncia == null)
                return NotFound();

            return Ok(denuncia);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DenunciaForm novaDenuncia)
        {
            var usuarioExiste = _context.Usuarios.Any(x => x.Id == novaDenuncia.UsuarioId);

            if (!usuarioExiste)
                return BadRequest("Usuario da denuncia nao encontrado.");

            var denuncia = new Denuncia
            {
                Descricao = novaDenuncia.Descricao,
                Categoria = novaDenuncia.Categoria,
                Endereco = novaDenuncia.Endereco,
                UsuarioId = novaDenuncia.UsuarioId,
                Status = string.IsNullOrWhiteSpace(novaDenuncia.Status) ? "Aberto" : novaDenuncia.Status
            };

            if (novaDenuncia.Imagem != null && novaDenuncia.Imagem.Length > 0)
            {
                var imagemUrl = await SalvarImagem(novaDenuncia.Imagem);

                if (imagemUrl == null)
                    return BadRequest("Envie uma imagem valida nos formatos JPG, PNG ou WEBP com ate 5 MB.");

                denuncia.ImagemUrl = imagemUrl;
            }

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
            denuncia.ImagemUrl = novaDenuncia.ImagemUrl ?? denuncia.ImagemUrl;

            _context.SaveChanges();

            return Ok(denuncia);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var denuncia = _context.Denuncias.Find(id);

            if (denuncia == null)
                return NotFound();

            RemoverImagem(denuncia.ImagemUrl);
            _context.Denuncias.Remove(denuncia);

            _context.SaveChanges();

            return Ok("Denuncia removida.");
        }

        [HttpGet("filter")]
        public IActionResult Filter(string? categoria, string? status, string? endereco)
        {
            var denuncias = ListarDenuncias();

            if (!string.IsNullOrWhiteSpace(categoria))
                denuncias = denuncias.Where(x => x.Categoria == categoria).ToList();

            if (!string.IsNullOrWhiteSpace(status))
                denuncias = denuncias.Where(x => x.Status == status).ToList();

            if (!string.IsNullOrWhiteSpace(endereco))
            {
                denuncias = denuncias.Where(x =>
                    x.Endereco != null &&
                    x.Endereco.Contains(endereco, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            return Ok(denuncias);
        }

        private List<DenunciaDetalhesDto> ListarDenuncias()
        {
            var denuncias = _context.Denuncias.ToList();
            var usuarios = _context.Usuarios.ToList();
            var resultado = new List<DenunciaDetalhesDto>();

            foreach (var denuncia in denuncias)
            {
                var usuario = usuarios.FirstOrDefault(x => x.Id == denuncia.UsuarioId);

                resultado.Add(new DenunciaDetalhesDto
                {
                    Id = denuncia.Id,
                    Endereco = denuncia.Endereco,
                    DataCriacao = denuncia.DataCriacao,
                    Categoria = denuncia.Categoria,
                    Descricao = denuncia.Descricao,
                    Status = denuncia.Status,
                    ImagemUrl = denuncia.ImagemUrl,
                    UsuarioId = denuncia.UsuarioId,
                    NomeUsuario = usuario?.Nome,
                    CpfUsuario = usuario?.Cpf
                });
            }

            return resultado;
        }

        public class DenunciaDetalhesDto
        {
            public int Id { get; set; }
            public string? Endereco { get; set; }
            public DateTime DataCriacao { get; set; }
            public string Categoria { get; set; } = string.Empty;
            public string? Descricao { get; set; }
            public string? Status { get; set; }
            public string? ImagemUrl { get; set; }
            public int UsuarioId { get; set; }
            public string? NomeUsuario { get; set; }
            public string? CpfUsuario { get; set; }
        }

        public class DenunciaForm
        {
            public string? Endereco { get; set; }
            public string Categoria { get; set; } = string.Empty;
            public string? Descricao { get; set; }
            public string? Status { get; set; }
            public int UsuarioId { get; set; }
            public IFormFile? Imagem { get; set; }
        }

        private async Task<string?> SalvarImagem(IFormFile imagem)
        {
            const long tamanhoMaximo = 5 * 1024 * 1024;
            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extensao = Path.GetExtension(imagem.FileName).ToLowerInvariant();

            if (imagem.Length > tamanhoMaximo ||
                !extensoesPermitidas.Contains(extensao))
            {
                return null;
            }

            var pastaUploads = ObterPastaUploads();
            Directory.CreateDirectory(pastaUploads);

            var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
            var caminhoArquivo = Path.Combine(pastaUploads, nomeArquivo);

            using var stream = new FileStream(caminhoArquivo, FileMode.Create);
            await imagem.CopyToAsync(stream);

            return $"/uploads/denuncias/{nomeArquivo}";
        }

        private void RemoverImagem(string? imagemUrl)
        {
            if (string.IsNullOrWhiteSpace(imagemUrl))
                return;

            var nomeArquivo = Path.GetFileName(imagemUrl);
            var caminhoArquivo = Path.Combine(ObterPastaUploads(), nomeArquivo);

            if (System.IO.File.Exists(caminhoArquivo))
            {
                System.IO.File.Delete(caminhoArquivo);
            }
        }

        private string ObterPastaUploads()
        {
            var wwwroot = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            return Path.Combine(wwwroot, "uploads", "denuncias");
        }
    }
}
