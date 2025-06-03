using System;
using System.Collections.Generic;
using System.Linq;
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
    public class PersonagensController : ControllerBase
    {
        private readonly RageModeApiContext _context;

        public PersonagensController(RageModeApiContext context)
        {
            _context = context;
        }

        // GET: api/Personagens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personagem>>> GetPersonagens()
        {
            var personagens = await _context.Personagens
     .Include(p => p.TipoPersonagem)
     .Include(p => p.Jogo)
     .ToListAsync();

            return Ok(personagens);
        }

        // GET: api/Personagens/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Personagem>> GetPersonagem(Guid id)
        {
            var personagem = await _context.Personagens.FindAsync(id);

            if (personagem == null)
            {
                return NotFound();
            }

            return personagem;
        }

        // PUT: api/Personagens/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonagem(Guid id, Personagem personagem)
        {
            if (id != personagem.PersonagemId)
            {
                return BadRequest();
            }

            _context.Entry(personagem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonagemExists(id))
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

        // POST: api/Personagens
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Personagem>> PostPersonagem(Personagem personagem)
        {
            _context.Personagens.Add(personagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonagem", new { id = personagem.PersonagemId }, personagem);


        }

        // DELETE: api/Personagens/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonagem(Guid id)
        {
            var personagem = await _context.Personagens.FindAsync(id);
            if (personagem == null)
            {
                return NotFound();
            }

            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonagemExists(Guid id)
        {
            return _context.Personagens.Any(e => e.PersonagemId == id);
        }
    }
}
