using System.ComponentModel.DataAnnotations;

namespace CidadeMelhorApi.Models
{
    public class Denuncia
    {
        public int Id { get; set; }
        public string? Endereco { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "A categoria e obrigatoria.")]
        public string Categoria { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Status { get; set; } = "Aberto";
        public string? ImagemUrl { get; set; }
        public int UsuarioId { get; set; }

    }
}
