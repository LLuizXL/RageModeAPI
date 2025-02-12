namespace RageModeAPI.Models
{
    public class Likes
    {
        public Guid LikeId { get; set; }
        public bool LikeorNot { get; set; }
        //chave estrangeira
        public Guid UsuarioId { get; set; }
        public Usuarios Usuarios { get; set; }
        public Guid PostId { get; set; }
        public Post Post { get; set; }

    }
}
