namespace RageModeAPI.Models
{
    public class Post
    {

        public string PostTitulo { get; set; }
        public string PostConteudo { get; set; }
        public string TipoPost { get; set; }
        public DateTime DataPostagem { get; set; }


        public Guid UsuarioId { get; set; }

        public Guid PostId { get; set; }

        public Guid JogoId { get; set; }
        public Guid PersonagemId { get; set; }


    }
}
