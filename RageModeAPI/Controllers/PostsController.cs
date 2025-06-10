using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        public PostsController(RageModeApiContext context)
        {
            _context = context;

        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            return await _context.Posts.Include(p => p.Usuarios) // Inclui o usuário relacionado
        .OrderByDescending(p => p.DataPostagem)
        .ToListAsync();
        }

        //Get: api/Posts/usuario/{usuarioId}
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByUsuario(Guid usuarioId)
        {
            return await _context.Posts
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Usuarios) // Inclui o usuário relacionado
                .OrderByDescending(p => p.DataPostagem)
                .ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(Guid id)
        {
            var post = await _context.Posts
       .Include(p => p.Usuarios)
       .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(
     Guid id,
     Post post,
     [FromServices] IAuthorizationService authorizationService)
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
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId))
                post.UsuarioId = Guid.Parse(userId);

            // Validação: PersonagemId existe?
            var personagemExiste = await _context.Personagens.AnyAsync(p => p.PersonagemId == post.PersonagemId);
            if (!personagemExiste)
                return BadRequest("PersonagemId informado não existe.");

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        }

        // DELETE: api/Posts/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(
      Guid id,
      [FromServices] IAuthorizationService authorizationService)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Chama a policy "AdminOrOwner" passando o id do post como recurso
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "AdminOrOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Posts/{postId}/like
        [HttpPost("{postId}/like")]
        public async Task<IActionResult> LikePost(Guid postId, [FromBody] bool like)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

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
        public async Task<IActionResult> UnlikePost(Guid postId)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

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
        public async Task<IActionResult> AddComment(Guid postId, [FromBody] Comentarios comentario)
        {
            var userId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            comentario.ComentariosId = Guid.NewGuid();
            comentario.PostId = postId;
            comentario.UsuarioId = userId;
            comentario.DataComentario = DateTime.UtcNow;

            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPostComments", new { postId = postId }, comentario);
        }

        // GET: api/Posts/{postId}/comments
        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comentarios>>> GetPostComments(Guid postId)
        {
            var comments = await _context.Comentarios
                .Where(c => c.PostId == postId)
                .ToListAsync();

            if (comments == null || comments.Count == 0)
            {
                return NotFound();
            }

            return comments;
        }

        // POST: api/Posts/{postId}/follow
        [HttpPost("{postId}/follow")]
        public async Task<IActionResult> FollowUserFromPost(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.Usuarios)
                .FirstOrDefaultAsync(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            var userId = post.UsuarioId;
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

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

            var newFollow = new Seguidores
            {
                SeguidoresId = Guid.NewGuid(),
                UsuarioId = currentUserId,
                SeguidoId = userId.Value // Corrigir para userId.Value
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
    }
}
