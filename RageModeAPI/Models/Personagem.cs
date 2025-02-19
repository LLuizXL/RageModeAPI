using System.ComponentModel.DataAnnotations;

namespace RageModeAPI.Models
{
    public class Personagem
    {
        public Guid PersonagemId { get; set; }

        [Required(ErrorMessage = "O nome do personagem é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do personagem não pode exceder 100 caracteres.")]
        public string PersonagemNome { get; set; }

        [StringLength(500, ErrorMessage = "A descrição do personagem não pode exceder 500 caracteres.")]
        public string PersonagemDescricao { get; set; }

        // Chave Estrangeira TipoPersonagem
        [Required(ErrorMessage = "O TipoPersonagemId é obrigatório.")]
        public Guid TipoPersonagemId { get; set; }
        public TipoPersonagem? TipoPersonagem { get; set; }

        // Chave Estrangeira Jogo
        public Guid JogoId { get; set; }
        public Jogos? Jogo { get; set; }
    }
}
