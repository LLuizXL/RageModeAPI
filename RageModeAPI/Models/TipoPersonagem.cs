using System.ComponentModel.DataAnnotations;

namespace RageModeAPI.Models
{
    public class TipoPersonagem
    {
        public Guid TipoPersonagemId { get; set; }

        [Required(ErrorMessage = "Insira o Estilo de jogo do Personagem.")]
        [MinLength(4, ErrorMessage = "O Estilo de jogo deve ter no mínimo 4 caracteres.")]
        public string TipoNome { get; set; }


        public ICollection<Personagem>? Personagens { get; set; }
    }
}
