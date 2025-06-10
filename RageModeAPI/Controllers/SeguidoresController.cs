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
    public class SeguidoresController : ControllerBase
    {
        private readonly RageModeApiContext _context;

        public SeguidoresController(RageModeApiContext context)
        {
            _context = context;
        }

        // GET: api/Seguidores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Seguidores>>> GetSeguidores()
        {
            return await _context.Seguidores.ToListAsync();
        }

        // GET: api/Seguidores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Seguidores>> GetSeguidores(Guid id)
        {
            var seguidores = await _context.Seguidores.FindAsync(id);

            if (seguidores == null)
            {
                return NotFound();
            }

            return seguidores;
        }

        // PUT: api/Seguidores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSeguidores(Guid id, Seguidores seguidores)
        {
            if (id != seguidores.SeguidoresId)
            {
                return BadRequest();
            }

            _context.Entry(seguidores).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeguidoresExists(id))
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

        // POST: api/Seguidores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Seguidores>> PostSeguidores(Seguidores seguidores)
        {
            _context.Seguidores.Add(seguidores);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSeguidores", new { id = seguidores.SeguidoresId }, seguidores);
        }

        // DELETE: api/Seguidores/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeguidores(Guid id)
        {
            var seguidores = await _context.Seguidores.FindAsync(id);
            if (seguidores == null)
            {
                return NotFound();
            }

            _context.Seguidores.Remove(seguidores);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SeguidoresExists(Guid id)
        {
            return _context.Seguidores.Any(e => e.SeguidoresId == id);
        }
    }
}
