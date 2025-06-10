using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RageModeAPI.Data;
using RageModeAPI.Models;

namespace RageModeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ComentariosController : ControllerBase
    {
        private readonly RageModeApiContext _context;

        public ComentariosController(RageModeApiContext context)
        {
            _context = context;
        }

        // GET: api/Comentarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comentarios>>> GetComentarios()
        {
            return await _context.Comentarios
                .Include(c => c.Usuario)
                .Include(c => c.Post)
                .ToListAsync();

        }


        // GET: api/Comentarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentarios>> GetComentarios(Guid id)
        {
            var comentarios = await _context.Comentarios
                .Include(c => c.Usuario)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.ComentariosId == id);

            if (comentarios == null)
            {
                return NotFound("Comentario não encontrado.");
            }

            return comentarios;
        }

        // PUT: api/Comentarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutComentarios(Guid id, [FromBody] ComentarioUpdateDto comentarioDto)
        {

            var comentarioExistente = await _context.Comentarios.FindAsync(id);

            if (comentarioExistente == null)
            {
                return NotFound("Comentario não encontrado.");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (comentarioExistente.UsuarioId != currentUserId)
            {
                return Forbid("Você não possui permissão para editar este comentário.");

            }

            comentarioExistente.ComentarioTexto = comentarioDto.ComentarioTexto;


            _context.Entry(comentarioExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ComentariosExists(id))
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

        //DTO da atualização do comentaario

        public class ComentarioUpdateDto
        {
            public string ComentarioTexto { get; set; }
        }

        // POST: api/Comentarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Comentarios>> PostComentarios([FromBody] ComentarioCreateDto comentarioDto)
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

            var post = await _context.Posts.FindAsync(comentarioDto.PostId);

            if (post == null)
            {
                return NotFound("Post não encontrado.");
            }

            var comentario = new Comentarios
            {
                ComentariosId = Guid.NewGuid(),
                ComentarioTexto = comentarioDto.ComentarioTexto,
                DataComentario = DateTime.UtcNow,
                UsuarioId = userId,
                PostId = comentarioDto.PostId
            };



            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();


            // Mostra o usuário e o post ao retornar o comentário que foi criado
            await _context.Entry(comentario)
                .Reference(c => c.Usuario)
                .LoadAsync();
            await _context.Entry(comentario)
                .Reference(c => c.Post)
                .LoadAsync();


            return CreatedAtAction(nameof(GetComentarios), new { id = comentario.ComentariosId }, comentario);
        }

        public class ComentarioCreateDto
        {
            public string ComentarioTexto { get; set; }
            public Guid PostId { get; set; }
        }

        // DELETE: api/Comentarios/5
        [Authorize]
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteComentarios(Guid id)
        {
            var comentarios = await _context.Comentarios.FindAsync(id);
            if (comentarios == null)
            {
                return NotFound("Comentario não encontrado.");
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comentarios.UsuarioId != currentUserId)
            {
                return Forbid("Você não possui permissão para excluir este comentário.");
            }

            // Policy: só admin ou o próprio autor do comentário pode deletar
            var authorizationResult = await authorizationService.AuthorizeAsync(User, comentarios.UsuarioId, "AdminOrOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _context.Comentarios.Remove(comentarios);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ComentariosExists(Guid id)
        {
            return _context.Comentarios.Any(e => e.ComentariosId == id);
        }

    }
}
