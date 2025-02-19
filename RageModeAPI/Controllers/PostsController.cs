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
            return await _context.Posts.ToListAsync();
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        // PUT: api/Posts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost(Guid id, Post post)
        {
            if (id != post.PostId)
            {
                return BadRequest();
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
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost(Post post)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            post.UsuarioId = Guid.Parse(userId);

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        }

        // DELETE: api/Posts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
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

        private bool PostExists(Guid id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
