using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CidadeMelhorApi.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string? Nome { get; set; }
        [EmailAddress(ErrorMessage = "O email é obrigatório.")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string? Senha { get; set; }

        [JsonIgnore]
        public string Discriminator { get; set; } = "Usuario";

    }
}
