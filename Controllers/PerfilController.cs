using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PerfilController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil == null)
                return NotFound("No se encontró el perfil del usuario.");

            return perfil.Rol switch
            {
                "Coleccionista" => View("PerfilColeccionista", perfil),
                "Perito" => View("PerfilPerito", perfil),
                "Restaurador" => View("PerfilRestaurador", perfil),
                _ => LocalRedirect("/")
            };
        }
    }
}