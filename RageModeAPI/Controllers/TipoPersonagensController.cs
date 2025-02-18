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
    public class TipoPersonagensController : ControllerBase
    {
        private readonly RageModeApiContext _context;

        public TipoPersonagensController(RageModeApiContext context)
        {
            _context = context;
        }

        // GET: api/TipoPersonagens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoPersonagem>>> GetTiposPersonagens()
        {
            return await _context.TiposPersonagens.ToListAsync();
        }

        // GET: api/TipoPersonagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoPersonagem>> GetTipoPersonagem(Guid id)
        {
            var tipoPersonagem = await _context.TiposPersonagens.FindAsync(id);

            if (tipoPersonagem == null)
            {
                return NotFound();
            }

            return tipoPersonagem;
        }

        // PUT: api/TipoPersonagens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoPersonagem(Guid id, TipoPersonagem tipoPersonagem)
        {
            if (id != tipoPersonagem.TipoPersonagemId)
            {
                return BadRequest();
            }

            _context.Entry(tipoPersonagem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoPersonagemExists(id))
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

        // POST: api/TipoPersonagens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoPersonagem>> PostTipoPersonagem(TipoPersonagem tipoPersonagem)
        {
            _context.TiposPersonagens.Add(tipoPersonagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoPersonagem", new { id = tipoPersonagem.TipoPersonagemId }, tipoPersonagem);
        }

        // DELETE: api/TipoPersonagens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoPersonagem(Guid id)
        {
            var tipoPersonagem = await _context.TiposPersonagens.FindAsync(id);
            if (tipoPersonagem == null)
            {
                return NotFound();
            }

            _context.TiposPersonagens.Remove(tipoPersonagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoPersonagemExists(Guid id)
        {
            return _context.TiposPersonagens.Any(e => e.TipoPersonagemId == id);
        }
    }
}
