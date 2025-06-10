using System.ComponentModel.DataAnnotations.Schema;

namespace RageModeAPI.Models
{
    public class Likes
    {
        public Guid LikesId { get; set; }
        public bool LikeorNot { get; set; }


        //Chave estrangeira para Usuário
        public string UsuariosId { get; set; }
        [ForeignKey("UsuariosId")]
        public Usuarios Usuarios { get; set; }

        //Chave estrangeira para Post
        public Guid PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }

    }
}
