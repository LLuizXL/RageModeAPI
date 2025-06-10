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
    public class UsuariosController : ControllerBase
    {
        private readonly RageModeApiContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(RageModeApiContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> GetUsuarios(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            // Pegue o IdentityUser relacionado
            var identityUser = await _userManager.FindByIdAsync(usuario.UserId.ToString());
            if (identityUser != null)
            {
                var roles = await _userManager.GetRolesAsync(identityUser);
                usuario.UsuarioRole = roles.FirstOrDefault();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(
      Guid id,
      Usuarios usuarios,
      [FromServices] IAuthorizationService authorizationService)
        {
            if (id != usuarios.UsuariosId)
            {
                return BadRequest();
            }

            // Policy: só admin ou o próprio usuário pode editar o perfil
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "AdminOrOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            // Verifique se o usuário existe antes de modificar
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Atualize apenas os campos permitidos
            usuarioExistente.UsuarioNome = usuarios.UsuarioNome;
            usuarioExistente.UsuarioEmail = usuarios.UsuarioEmail;
            // ...atualize outros campos conforme necessário...

            _context.Entry(usuarioExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuariosExists(id))
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

        private bool UsuariosExists(Guid id)
        {
            return _context.Usuarios.Any(e => e.UsuariosId == id);
        }
        // GET: api/Usuarios/{userId}/followers/count
        [HttpGet("{userId}/followers/count")]
        public async Task<ActionResult<int>> GetFollowerCount(Guid userId)
        {
            var user = await _context.Usuarios
                .Include(u => u.Seguidores)
                .FirstOrDefaultAsync(u => u.UsuariosId == userId);

            if (user == null)
            {
                return NotFound();
            }

            return user.FollowerCount;
        }

        // POST: api/Usuarios/{userId}/follow
        [Authorize]
        [HttpPost("{userId}/follow")]
        public async Task<IActionResult> FollowUser(Guid userId)
        {
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
                SeguidoId = userId
            };

            _context.Seguidores.Add(newFollow);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        //Delete : api/Usuarios/{userId}/unfollow
        [Authorize]
        [HttpDelete("{userId}/unfollow")]
        public async Task<IActionResult> UnfollowUser(Guid userId)
        {
            var currentUserId = Guid.Parse(User.Claims.FirstOrDefault(c => c.Type == "UserId").Value);

            var existingFollow = await _context.Seguidores
                .FirstOrDefaultAsync(f => f.UsuarioId == currentUserId && f.SeguidoId == userId);

            if (existingFollow == null)
            {
                return BadRequest("Você não está seguindo este usuário.");
            }

            _context.Seguidores.Remove(existingFollow);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuarios>> PostUsuarios(Usuarios usuarios)
        {
            //Pegar o Id do Usuario Logado
            // Obter o ID do usuário de forma mais segura
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            usuarios.UsuariosId = Guid.Parse(userId);

            var user = await _userManager.FindByIdAsync(userId);
            usuarios.UsuarioEmail = user.Email;
            usuarios.UsuarioSenha = user.PasswordHash;


            _context.Usuarios.Add(usuarios);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuarios", new { id = usuarios.UsuariosId }, usuarios);
        }

        // DELETE: api/Usuarios/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(
     Guid id,
     [FromServices] IAuthorizationService authorizationService)
        {
            var usuarios = await _context.Usuarios.FindAsync(id);
            if (usuarios == null)
            {
                return NotFound();
            }

            // Policy: só admin ou o próprio usuário pode deletar a conta
            var authorizationResult = await authorizationService.AuthorizeAsync(User, id, "AdminOrOwner");
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            _context.Usuarios.Remove(usuarios);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Usuarios/{userId}/addrole
        [HttpPost("{userId}/addrole")]
        public async Task<IActionResult> AddRoleToUser(Guid userId, [FromBody] string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return NotFound("Usuário não encontrado.");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (result.Succeeded)
                return Ok("Role adicionada com sucesso!");
            else
                return BadRequest(result.Errors);
        }
    }
}
