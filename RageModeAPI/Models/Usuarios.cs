﻿using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RageModeAPI.Models
{
    public class Usuarios
    {
        public Guid UsuariosId { get; set; }
        [Required(ErrorMessage = "O nome do usuário é obrigatório.")]
        [MaxLength(50, ErrorMessage = "O nome do usuário deve ter no máximo 20 caracteres.")]
        public string UsuarioNome { get; set; }

        public string? UsuarioEmail { get; set; }
        public string? UsuarioSenha { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Post>? Posts { get; set; }
        public ICollection<Likes>? Likes { get; set; }
        public ICollection<Comentarios>? Comentarios { get; set; }
        public ICollection<Seguidores>? Seguindo { get; set; }
        public ICollection<Seguidores>? Seguidores { get; set; }

        //Não será mapeado pra tabela a contagem de seguidores
        [NotMapped]
        public int FollowerCount => Seguidores?.Count ?? 0;

        public Guid? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
