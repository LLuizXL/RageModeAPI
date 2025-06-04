using System.ComponentModel.DataAnnotations.Schema;

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
        public Guid? UsuarioId { get; set; }
        public Usuarios? Usuarios { get; set; }

        public Guid PersonagemId { get; set; }
        public Personagem? Personagem { get; set; }
        public ICollection<Likes>? Likes { get; set; }
        public ICollection<Comentarios>? Comentarios { get; set; }

        [NotMapped]
        public int LikeCount => Likes?.Count(l => l.LikeorNot) ?? 0;
        [NotMapped]
        public int DislikeCount => Likes?.Count(l => !l.LikeorNot) ?? 0;

        [NotMapped]
        public int CommentCount => Comentarios?.Count ?? 0; // Calcula o número de comentários


    }
}
