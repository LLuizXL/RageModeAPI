using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class UsuariosController : ControllerBase
    {
        private readonly RageModeApiContext _context;
        private readonly UserManager<Usuarios> _userManager;
        private readonly IAuthorizationService authorizationService;

        public UsuariosController(RageModeApiContext context, UserManager<Usuarios> userManager, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            this.authorizationService = authorizationService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuarios>>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        //[HttpGet("UserById")]
        //public async Task<ActionResult<Usuarios>> GetUsuarios(string id)
        //{
        //    var usuario = await _context.Usuarios.FindAsync(id);
        //    if (usuario == null)
        //        return NotFound();

        //    // Pegue o IdentityUser relacionado
        //    var identityUser = await _userManager.FindByIdAsync(usuario.UserId.ToString());
        //    if (identityUser != null)
        //    {
        //        var roles = await _userManager.GetRolesAsync(identityUser);
        //        usuario.UsuarioRole = roles.FirstOrDefault();
        //    }

        //    return usuario;
        //}

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(
      string id,
      Usuarios usuarios,
      [FromServices] IAuthorizationService authorizationService)
        {
            if (id != usuarios.Id)
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
            usuarioExistente.Email = usuarios.Email;
            // ...atualize outros campos conforme necessário...

            _context.Entry(usuarioExistente).State = EntityState.Modified;



            var userExistente = await _userManager.FindByIdAsync(id);

            if (userExistente == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            userExistente.UserName = usuarios.UsuarioNome;
            userExistente.Email = usuarios.Email;

            var result = await _userManager.UpdateAsync(userExistente);

            if (!result.Succeeded)
            {
                return BadRequest("Erro ao atualizar o usuário. " + result.Errors);
            }

            return NoContent();
        }
        // GET: api/Usuarios/{userId}/followers/count
        [HttpGet("{userId}/followers/count")]
        public async Task<ActionResult<int>> GetFollowerCount(string userId)
        {
            var user = await _context.Usuarios
                .Include(u => u.Seguidores)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return user.FollowerCount;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model) // Use um DTO para o registro
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Crie uma nova instância de Usuarios
            var user = new Usuarios
            {
                UserName = model.Email, // IdentityUser.UserName é muitas vezes o email para login
                Email = model.Email,
                UsuarioNome = model.UsuarioNome, // Seu nome de exibição
                CreatedAt = DateTime.Now,
                EmailConfirmed = true // Para testes, pode ser true. Em produção, envie email de confirmação
            };

            // Crie o usuário com a senha usando UserManager
            var result = await _userManager.CreateAsync(user, model.Senha);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }



            // Retorne o usuário criado (sem a senha)
            return CreatedAtAction(nameof(GetUsuarios), new { id = user.Id }, new UserResponseDto // Utilizando o .Id e um DTO de resposta
            {
                Id = user.Id,
                Email = user.Email,
                UsuarioNome = user.UsuarioNome,
                CreatedAt = DateTime.Now
            });
        }

        // DTO pra criar o usuariol
        public class RegisterUserDto
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [DataType(DataType.Password)]
            public string Senha { get; set; }

            [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
            [StringLength(50, ErrorMessage = "O nome de exibição deve ter no máximo 50 caracteres.")]
            public string UsuarioNome { get; set; }
        }

        // DTO para a resposta do usuário la no body response
        public class UserResponseDto
        {
            public string Id { get; set; } // Id do identity é string então vamo usar string aqui
            public string Email { get; set; }
            public string UsuarioNome { get; set; }
            public DateTime CreatedAt { get; set; }
        }


        //Agora é hora que o filho chora e a mãe nao ve

        // POST: api/Usuarios/{userId}/follow
        [Authorize]
        [HttpPost("{userId}/follow")]
        public async Task<IActionResult> FollowUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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
        // Não vai usar esse endpoint pq ta repetido e tamo usando o userIdentity pra criar usuario ali em cima (herdando do Identity)
        //[HttpPost]
        //public async Task<ActionResult<Usuarios>> PostUsuarios(Usuarios usuarios)
        //{
        //    //Pegar o Id do Usuario Logado
        //    // Obter o ID do usuário de forma mais segura
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    usuarios.UsuariosId = Guid.Parse(userId);

        //    var user = await _userManager.FindByIdAsync(userId);
        //    usuarios.UsuarioEmail = user.Email;
        //    usuarios.UsuarioSenha = user.PasswordHash;


        //    _context.Usuarios.Add(usuarios);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUsuarios", new { id = usuarios.UsuariosId }, usuarios);
        //}

        //^^^ Vou deixar comentado pq vai q precisa dps né, nunca se sabe


        // DELETE: api/Usuarios/5
        //O usuário só vai poder deletar, se o Id for o mesmo do usuario logado OU se ele for administrador
        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUsuarios(string id)
        {

            var isAdmin = User.IsInRole("Admin");
            var atualUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != atualUserId && !isAdmin)
            {
                return Forbid("Você não tem permissão para excluir este usuário.");
            }
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
        public async Task<IActionResult> AddRoleToUser(string userId, [FromBody] string role)
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
