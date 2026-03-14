using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;

namespace PGDCP.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext ctx, UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
        { _context = ctx; _userManager = um; _roleManager = rm; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalObras          = await _context.Obras.CountAsync();
            ViewBag.TotalConservaciones = await _context.Conservaciones.CountAsync();
            ViewBag.TotalValoraciones   = await _context.Valoraciones.CountAsync();
            ViewBag.ValorTotal          = await _context.Obras.SumAsync(o => (decimal?)o.ValorEstimado) ?? 0;
            ViewBag.TotalUsuarios       = _userManager.Users.Count();
            return View();
        }

        public async Task<IActionResult> Usuarios()
        {
            var users = _userManager.Users.ToList();
            var list  = new List<(IdentityUser u, IList<string> roles)>();
            foreach (var u in users) list.Add((u, await _userManager.GetRolesAsync(u)));
            ViewBag.UsersWithRoles = list;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRol(string userId, string rol)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var current = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, current);
            await _userManager.AddToRoleAsync(user, rol);
            TempData["Success"] = $"Rol '{rol}' asignado a {user.Email}";
            return RedirectToAction(nameof(Usuarios));
        }
    }
}
