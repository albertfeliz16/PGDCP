using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext ctx,
            UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
        { _context = ctx; _userManager = um; _roleManager = rm; }

        // Panel principal con estadisticas
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalObras = await _context.Obras.CountAsync();
            ViewBag.TotalConservaciones = await _context.Conservaciones.CountAsync();
            ViewBag.TotalValoraciones = await _context.Valoraciones.CountAsync();
            ViewBag.ValorTotal = await _context.Obras
                .SumAsync(o => (decimal?)o.ValorEstimado) ?? 0;
            ViewBag.TotalUsuarios = _userManager.Users.Count();
            return View();
        }

        // Lista de todos los usuarios con sus roles
        public async Task<IActionResult> Usuarios()
        {
            var users = _userManager.Users.ToList();
            var list = new List<(IdentityUser u, IList<string> roles)>();
            foreach (var u in users)
                list.Add((u, await _userManager.GetRolesAsync(u)));
            ViewBag.UsersWithRoles = list;
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
            return View();
        }

        // Mostrar formulario de edicion de un usuario (GET)
        public async Task<IActionResult> EditarUsuario(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userId);
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name!).ToList();
            ViewBag.RolActual = roles.FirstOrDefault() ?? "";
            ViewBag.User = user;
            ViewBag.Perfil = perfil;
            return View(perfil);
        }

        // Guardar cambios de edicion del usuario (POST)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(string userId,
            string nombre, string apellido, string telefono,
            string sexo, string rol, DateTime fechaNacimiento)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (perfil != null)
            {
                perfil.Nombre = nombre;
                perfil.Apellido = apellido;
                perfil.Telefono = telefono;
                perfil.Sexo = sexo;
                perfil.Rol = rol;
                perfil.FechaNacimiento = fechaNacimiento;
                _context.Update(perfil);
                await _context.SaveChangesAsync();
            }
            var rolesActuales = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, rolesActuales);
            await _userManager.AddToRoleAsync(user, rol);
            TempData["Success"] = $"Usuario {user.Email} actualizado.";
            return RedirectToAction(nameof(Usuarios));
        }

        // Asignar rol rapido desde la lista (POST)
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

        // Confirmar eliminacion (GET)
        public async Task<IActionResult> EliminarUsuario(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userId);
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.User = user;
            ViewBag.Perfil = perfil;
            ViewBag.Rol = roles.FirstOrDefault() ?? "Sin rol";
            return View();
        }

        // Ejecutar eliminacion (POST)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarEliminar(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();
            var usuarioActual = await _userManager.GetUserAsync(User);
            if (usuarioActual?.Id == userId)
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction(nameof(Usuarios));
            }
            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (perfil != null)
            {
                _context.PerfilesUsuario.Remove(perfil);
                await _context.SaveChangesAsync();
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                TempData["Success"] = $"Usuario {user.Email} eliminado.";
            else
                TempData["Error"] = "No se pudo eliminar el usuario.";
            return RedirectToAction(nameof(Usuarios));
        }
    }
}

