namespace RageModeAPI.Models
{
    public class Seguidores
    {
        public Guid SeguirId { get; set; }

        //Chave Estrangeira Usuario
        public Guid UsuarioId { get; set; }
        public Usuarios? Usuario { get; set; }

        //Chave Estrangeira(Usuario) Seguido
        public Guid SeguidoId { get; set; }
        public Usuarios? Seguido { get; set; }

    }
}
