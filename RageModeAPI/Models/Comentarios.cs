namespace RageModeAPI.Models
{
    public class Comentarios
    {
        public Guid ComentarioId { get; set; }
        public string ComentarioTexto { get; set; }
        public DateTime DataComentario { get; set; }
        //Chave Estrangeira Usuario
        public Guid UsuarioId { get; set; }
        public Usuarios? Usuario { get; set; }

        //Chave Estrangeira Post
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
    }
}
