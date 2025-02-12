namespace RageModeAPI.Models
{
    public class Post
    {

        public Guid PostId { get; set; }
        public string PostTitulo { get; set; }
        public string PostConteudo { get; set; }
        public string TipoPost { get; set; }
        public DateTime DataPostagem { get; set; }

        //ChavesEstrangeiras 
        public Guid UsuarioId { get; set; }
        public Usuarios? Usuario { get; set; }


        public Guid JogoId { get; set; }
        public Jogos? Jogo { get; set; }
        public Guid PersonagemId { get; set; }
        public Personagem? Personagem { get; set; }


    }
}
