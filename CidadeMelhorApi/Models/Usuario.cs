using System.ComponentModel.DataAnnotations;

namespace CidadeMelhorApi.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome e obrigatorio.")]
        public string? Nome { get; set; }

        [EmailAddress(ErrorMessage = "Digite um email valido.")]
        public string? Email { get; set; }

        public string? Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha e obrigatoria.")]
        public string? Senha { get; set; }
    }
}
