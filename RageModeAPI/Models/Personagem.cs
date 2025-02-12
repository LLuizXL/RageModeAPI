namespace RageModeAPI.Models
{
    public class Personagem
    {
        public Guid PersonagemId { get; set; }
        public string PersonagemNome { get; set; }
        public string PersonagemDescricao { get; set; }

        //Chave Estrangeira TipoPersonagem
        public Guid TipoPersonagemId { get; set; }
        public TipoPersonagem? TipoPersonagem { get; set; }

        //Chave Estrangeira Jogo
        public Guid JogoId { get; set; }
        public Jogos? Jogo { get; set; }
    }
}
