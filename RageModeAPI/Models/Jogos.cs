using System.ComponentModel.DataAnnotations;

namespace RageModeAPI.Models
{
    public class Jogos
    {
        public Guid JogosId { get; set; }

        [Required(ErrorMessage = "O nome do jogo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do jogo não pode exceder 100 caracteres.")]
        public string JogoNome { get; set; }
        public string imageBanner { get; set; }

        [StringLength(500, ErrorMessage = "A descrição do jogo não pode exceder 500 caracteres.")]
        public string JogoDescricao { get; set; }

        [Required(ErrorMessage = "O ano de lançamento é obrigatório.")]
        public DateOnly AnoLancamento { get; set; }
    }
}
