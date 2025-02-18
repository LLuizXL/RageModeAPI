using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RageModeAPI.Data;
using RageModeAPI.Models;

namespace RageModeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentarios(Guid id, Comentarios comentarios)
        {
            if (id != comentarios.ComentariosId)
            {
                return BadRequest();
            }

            _context.Entry(comentarios).State = EntityState.Modified;

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
        [HttpPost]
        public async Task<ActionResult<Comentarios>> PostComentarios(Comentarios comentarios)
        {
            _context.Comentarios.Add(comentarios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComentarios", new { id = comentarios.ComentariosId }, comentarios);
        }

        // DELETE: api/Comentarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentarios(Guid id)
        {
            var comentarios = await _context.Comentarios.FindAsync(id);
            if (comentarios == null)
            {
                return NotFound();
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
