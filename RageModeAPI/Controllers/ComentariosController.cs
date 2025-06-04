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
            return await _context.Comentarios.ToListAsync();
        }

        // GET: api/Comentarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentarios>> GetComentarios(Guid id)
        {
            var comentarios = await _context.Comentarios.FindAsync(id);

            if (comentarios == null)
            {
                return NotFound();
            }

            return comentarios;
        }

        // PUT: api/Comentarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentarios(
       Guid id,
       Comentarios comentarios,
       [FromServices] IAuthorizationService authorizationService)
        {
            if (id != comentarios.ComentariosId)
            {
                return BadRequest();
            }

            // Busca o comentário existente para pegar o autor
            var comentarioExistente = await _context.Comentarios.FindAsync(id);
            if (comentarioExistente == null)
            {
                return NotFound();
            }

            // Policy: só admin ou o próprio autor pode editar
            var authorizationResult = await authorizationService.AuthorizeAsync(User, comentarioExistente.UsuarioId, "AdminOrOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            // Atualize apenas os campos permitidos
            comentarioExistente.ComentarioTexto = comentarios.ComentarioTexto;
            // ...atualize outros campos conforme necessário...

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

        // POST: api/Comentarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Comentarios>> PostComentarios(Comentarios comentarios)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            comentarios.UsuarioId = Guid.Parse(userId);
            _context.Comentarios.Add(comentarios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComentarios", new { id = comentarios.ComentariosId }, comentarios);
        }

        // DELETE: api/Comentarios/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentarios(
     Guid id,
     [FromServices] IAuthorizationService authorizationService)
        {
            var comentarios = await _context.Comentarios.FindAsync(id);
            if (comentarios == null)
            {
                return NotFound();
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
