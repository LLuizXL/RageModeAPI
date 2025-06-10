using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RageModeAPI.Models
{
    public class Seguidores
    {
        [Key]
        public Guid SeguidoresId { get; set; }

        //Chave Estrangeira Usuario
        public string UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuarios? Usuario { get; set; }

        //Chave Estrangeira(Usuario) Seguido
        public string SeguidoId { get; set; }


        [ForeignKey("SeguidoId")]
        public Usuarios? Seguido { get; set; }

    }
}
