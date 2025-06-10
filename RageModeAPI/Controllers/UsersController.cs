using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace RageModeAPI.Controllers
{
    [Route("Users")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Users
        [HttpGet]
        public ActionResult<IEnumerable<object>> GetIdentityUsers()
        {
            var users = _userManager.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email
                })
                .ToList();

            return Ok(users);
        }
    }
}
