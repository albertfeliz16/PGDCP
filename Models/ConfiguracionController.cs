using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize]
    public class ConfiguracionController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ConfiguracionController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // --- VISTA MI PERFIL ---
        [HttpGet]
        public async Task<IActionResult> MiPerfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var perfil = await _context.PerfilesUsuario.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var model = new PerfilUsuarioViewModel
            {
                Nombre = perfil?.Nombre ?? "",
                Apellido = perfil?.Apellido ?? "",
                FechaNacimiento = perfil?.FechaNacimiento ?? DateTime.Today,
                Sexo = perfil?.Sexo ?? "",
                Telefono = perfil?.Telefono ?? ""
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MiPerfil(PerfilUsuarioViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var perfil = await _context.PerfilesUsuario.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (perfil == null)
            {
                perfil = new PerfilUsuario { UserId = user.Id };
                _context.PerfilesUsuario.Add(perfil);
            }

            perfil.Nombre = model.Nombre;
            perfil.Apellido = model.Apellido;
            perfil.FechaNacimiento = model.FechaNacimiento;
            perfil.Sexo = model.Sexo;
            perfil.Telefono = model.Telefono;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(MiPerfil));
        }

        // --- VISTA SEGURIDAD ---
        [HttpGet]
        public async Task<IActionResult> Seguridad()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            return View(new SeguridadViewModel { Email = user.Email ?? "" });
        }

        [HttpPost]
        public async Task<IActionResult> Seguridad(SeguridadViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (model.Email != user.Email)
            {
                await _userManager.SetEmailAsync(user, model.Email);
                await _userManager.SetUserNameAsync(user, model.Email);
            }

            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
                    return View(model);
                }
            }

            TempData["Success"] = "Credenciales actualizadas.";
            return RedirectToAction(nameof(Seguridad));
        }
    }
}