namespace RageModeAPI.Models
{
    public class Jogos
    {
        public Guid JogosId { get; set; }
        public string JogoNome { get; set; }
        public string JogoDescricao { get; set; }
        public DateOnly AnoLancamento { get; set; }
    }
}
