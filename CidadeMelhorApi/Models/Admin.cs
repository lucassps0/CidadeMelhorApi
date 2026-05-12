using System.ComponentModel.DataAnnotations;

namespace CidadeMelhorApi.Models
{
    public class Admin
    {
        public int Id { get; set; }


        [Required(
        ErrorMessage = "O nome do admin é obrigatório."
        )]
        public string Nome { get; set; }


        [EmailAddress(
        ErrorMessage = "O e-mail é obrigatório."
        )]
        public string Email { get; set; }


        [Required(
        ErrorMessage = "A senha é obrigatória."
        )]
        public string Senha { get; set; }
    }
}