using System.ComponentModel.DataAnnotations;

namespace CidadeMelhorApi.Models
{
    public class Admin
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do admin e obrigatorio.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email e obrigatorio.")]
        [EmailAddress(ErrorMessage = "Digite um email valido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha e obrigatoria.")]
        public string Senha { get; set; } = string.Empty;
    }
}
