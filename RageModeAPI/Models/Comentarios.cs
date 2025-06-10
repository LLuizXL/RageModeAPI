using System.ComponentModel.DataAnnotations.Schema;

namespace RageModeAPI.Models
{
    public class Comentarios
    {

        public Guid ComentariosId { get; set; }
        public string ComentarioTexto { get; set; }
        public DateTime DataComentario { get; set; }


        //Chave Estrangeira Usuario
        public string UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuarios? Usuario { get; set; }
        //Chave Estrangeira Post
        public Guid PostId { get; set; }
        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}
