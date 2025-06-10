using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RageModeAPI.Models; // Certifique-se de que sua model Usuarios está aqui
using RageModeAPI.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RageModeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Isso fará com que os endpoints comecem com /api/Auth
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Usuarios> _userManager;
        private readonly SignInManager<Usuarios> _signInManager;
        private readonly ITokenService _tokenService; // Serviço para gerar JWTs
        // Se você já tem um serviço para gerar JWTs (TokenService, JwtService, etc.), injete-o aqui:
        // private readonly ITokenService _tokenService;

        public AuthController(UserManager<Usuarios> userManager,
                              SignInManager<Usuarios> signInManager,
                               ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registra um novo usuário no sistema.
        /// </summary>
        [HttpPost("register")] // Rota: POST /api/Auth/register
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new Usuarios
            {
                UserName = model.Email, // O UserName do Identity é frequentemente o email
                Email = model.Email,
                UsuarioNome = model.UsuarioNome, // Sua propriedade customizada
                CreatedAt = DateTime.UtcNow // Sua propriedade customizada
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Opcional: Adicionar o usuário a um Role padrão aqui, se você tiver roles
                // await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { Message = "User registered successfully.", UserId = user.Id });
            }

            // Se falhou, retorna os erros do Identity
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Realiza o login do usuário e retorna um token de autenticação.
        /// </summary>
        [HttpPost("login")] // Rota: POST /api/Auth/login
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid credentials." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // *** IMPORTANTE: Aqui você deve gerar seu JWT (JSON Web Token) ***
                // Este é um placeholder, você precisa de um serviço de token para isso.
                // Exemplo (se você tivesse um ITokenService injetado):
                var token = _tokenService.GenerateJwtToken(user);
                return Ok(new { Token = token, UserId = user.Id, UserName = user.UserName, DisplayName = user.UsuarioNome });

            }

            return Unauthorized(new { Message = "Invalid credentials." });
        }

        // Modelos de requisição para registro e login
        public class RegisterModel
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [StringLength(100, ErrorMessage = "A {0} deve ter pelo menos {2} e no máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirme a senha")]
            [Compare("Password", ErrorMessage = "A senha e a confirmação de senha não coincidem.")]
            public string ConfirmPassword { get; set; }

            [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
            public string UsuarioNome { get; set; } // Sua propriedade customizada
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "A senha é obrigatória.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}