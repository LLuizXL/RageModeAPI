using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace RageModeAPI.Data.Authorization
{
    public class AdminOrOwnerHandler : AuthorizationHandler<AdminOrOwnerRequirement, Guid>
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RageModeApiContext _context;

        public AdminOrOwnerHandler(UserManager<IdentityUser> userManager, RageModeApiContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminOrOwnerRequirement requirement,
            Guid resourceId)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return;

            var isAdmin = await _userManager.IsInRoleAsync(user, "admin");
            if (isAdmin)
            {
                context.Succeed(requirement);
                return;
            }

            // Exemplo para Post: verifica se o usuário é o dono do recurso
            var post = await _context.Posts.FindAsync(resourceId);
            if (post != null && post.UsuarioId.ToString() == userId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
