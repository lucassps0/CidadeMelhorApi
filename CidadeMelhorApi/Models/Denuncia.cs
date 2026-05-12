using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CidadeMelhorApi.Models
{
    public class Denuncia
    {
        public int Id { get; set; }
        public string? Endereco { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [RegularExpression(
        @"^(Buraco na via|Vazamento|Lixo Acumulado|Iluminacao publica|Sinalizacao|outros)$",
        ErrorMessage = "A categoria deve ser: Buraco na via, Vazamento, Lixo Acumulado, Iluminacao publica, Sinalizacao ou Outros."
        )]
        public string Categoria { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? Status { get; set; } = "Aberto";
        public int UsuarioId { get; set; }

    }
}