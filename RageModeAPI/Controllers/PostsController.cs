using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RageModeAPI.Data;
using RageModeAPI.Models;

namespace RageModeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PostsController : ControllerBase
    {
        private readonly RageModeApiContext _context;
        private readonly UserManager<Usuarios> _userManager;
        private readonly IAuthorizationService authorizationService;

        public PostsController(RageModeApiContext context, UserManager<Usuarios> userManager, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            this.authorizationService = authorizationService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetPosts()
        {
            return await _context.Posts
                .Include(p => p.Usuarios)
                .Select(p => new PostDto
                {
                    PostId = p.PostId,
                    PostTitulo = p.PostTitulo,
                    PostConteudo = p.PostConteudo,
                    TipoPost = p.TipoPost,
                    DataPostagem = p.DataPostagem,
                    UsuarioNome = p.Usuarios.UsuarioNome
                })
                .ToListAsync();
        }

        //Get: api/Posts/usuario/{usuarioId}
        [HttpGet("PostPorUsuarioId")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByUsuario(string usuarioId)
        {
            return await _context.Posts
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Usuarios) // Inclui o usuário relacionado
                .OrderByDescending(p => p.DataPostagem)
                .ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id)
        {
            var post = await _context.Posts
                .Include(p => p.Usuarios)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            var postDto = new PostDto
            {
                PostId = post.PostId,
                PostTitulo = post.PostTitulo,
                PostConteudo = post.PostConteudo,
                TipoPost = post.TipoPost,
                DataPostagem = post.DataPostagem,
                UsuarioNome = post.Usuarios?.UsuarioNome
            };

            return postDto;
        }

        // PUT: api/Posts/5
        [Authorize]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutPost(Guid id, Post post)
        {
            if (id != post.PostId)
            {
                return BadRequest();
            }

            // Policy: só admin ou dono do post pode editar
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "AdminOrOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Posts
        [Authorize]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] PostCreateDto newPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var post = new Post
            {
                PostTitulo = newPostDto.PostTitulo,
                PostConteudo = newPostDto.PostConteudo,
                TipoPost = newPostDto.TipoPost,
                DataPostagem = DateTime.UtcNow,
                UsuarioId = userId,
                PersonagemId = newPostDto.PersonagemId
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            await _context.Entry(post).Reference(p => p.Usuarios).LoadAsync();

            var postDto = new PostDto
            {
                PostId = post.PostId,
                PostTitulo = post.PostTitulo,
                PostConteudo = post.PostConteudo,
                TipoPost = post.TipoPost,
                DataPostagem = post.DataPostagem,
                UsuarioNome = post.Usuarios?.UsuarioNome
            };

            return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, postDto);
        }

        // DELETE: api/Posts/5
        [Authorize]
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var isAdmin = User.IsInRole("Admin");
            var atualUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Verifica se o usuário é o dono do post ou um administrador
            if (post.UsuarioId != atualUserId && !isAdmin)
            {
                return Forbid("Você não tem autorização para excluir este post.");
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Posts/{postId}/like
        [HttpPost("{postId}/like")]
        [Authorize]
        public async Task<IActionResult> LikePost(Guid postId, [FromBody] bool like)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UsuariosId == userId);

            if (existingLike != null)
            {
                existingLike.LikeorNot = like;
                _context.Entry(existingLike).State = EntityState.Modified;
            }
            else
            {
                var newLike = new Likes
                {
                    LikesId = Guid.NewGuid(),
                    LikeorNot = like,
                    UsuariosId = userId,
                    PostId = postId
                };
                _context.Likes.Add(newLike);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Posts/{postId}/like
        [HttpDelete("{postId}/like")]
        [Authorize]
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UsuariosId == userId);

            if (existingLike == null)
            {
                return NotFound();
            }

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Posts/{postId}/comment
        [HttpPost("{postId}/comment")]
        [Authorize]
        public async Task<IActionResult> AddComment(Guid postId, [FromBody] Comentarios comentario)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            comentario.ComentariosId = Guid.NewGuid();
            comentario.PostId = postId;
            comentario.UsuarioId = userId;
            comentario.DataComentario = DateTime.UtcNow;

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            // Retorne apenas o id do comentário criado, para evitar ciclos
            return CreatedAtAction("GetPostComments", new { postId = postId }, new { comentario.ComentariosId });
        }

        // GET: api/Posts/{postId}/comments
        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<PostComentarioDto>>> GetPostComments(Guid postId)
        {
            var comments = await _context.Comentarios
                .Where(c => c.PostId == postId)
                .Include(c => c.Usuario)
                .Select(c => new PostComentarioDto
                {
                    ComentariosId = c.ComentariosId,
                    ComentarioTexto = c.ComentarioTexto,
                    DataComentario = c.DataComentario,
                    UsuarioNome = c.Usuario.UsuarioNome
                })
                .ToListAsync();

            if (comments == null || comments.Count == 0)
            {
                return NotFound("Não há comentários para esse post.");
            }

            return comments;
        }

        // POST: api/Posts/{postId}/follow
        [HttpPost("{postId}/follow")]
        [Authorize]
        public async Task<IActionResult> FollowUserFromPost(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.Usuarios)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound("Post nao encontrado.");
            }

            var userId = post.UsuarioId;
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userIdToFollow = post.UsuarioId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Usuário não autenticado.");
            }

            if (currentUserId == userId)
            {
                return BadRequest("Você não pode seguir a si mesmo.");
            }

            var existingFollow = await _context.Seguidores
                .FirstOrDefaultAsync(f => f.UsuarioId == currentUserId && f.SeguidoId == userId);

            if (existingFollow != null)
            {
                return BadRequest("Você já está seguindo este usuário.");
            }

            var usuarioAlvo = await _userManager.FindByIdAsync(userIdToFollow);

            if (usuarioAlvo == null)
            {
                return NotFound("Usuário a ser seguido não encontrado.");
            }

            var newFollow = new Seguidores
            {
                SeguidoresId = Guid.NewGuid(),
                UsuarioId = currentUserId, // Id Seguidor
                SeguidoId = userIdToFollow // Id Seguido
            };

            _context.Seguidores.Add(newFollow);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{postId}/upload-image")]
        public async Task<IActionResult> UploadImage(Guid postId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("O arquivo de imagem é obrigatório.");

            if (!file.ContentType.StartsWith("image/"))
                return BadRequest("O arquivo deve ser uma imagem.");

            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
                return NotFound("Post não encontrado.");

            var directoryPath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var fileName = $"{postId}_{file.FileName}";
            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            post.ImageUrl = $"/images/{fileName}";
            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { PostId = post.PostId, ImageUrl = post.ImageUrl });
        }

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        // DTOs
        public class PostDto
        {
            public Guid PostId { get; set; }
            public string PostTitulo { get; set; }
            public string PostConteudo { get; set; }
            public string TipoPost { get; set; }
            public DateTime DataPostagem { get; set; }
            public string UsuarioNome { get; set; }
        }

        public class PostCreateDto
        {
            public string PostTitulo { get; set; }
            public string PostConteudo { get; set; }
            public string TipoPost { get; set; }
            public Guid PersonagemId { get; set; }
        }

        public class PostComentarioDto
        {
            public Guid ComentariosId { get; set; }
            public string ComentarioTexto { get; set; }
            public DateTime DataComentario { get; set; }
            public string UsuarioNome { get; set; }
        }
    }
}
