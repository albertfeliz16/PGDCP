#nullable disable
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        private const int MaxIntentos = 5;
        private const int MinutosBloqueo = 15;

        public LoginController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Perfil");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string email, string password, bool recordarme = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "El correo y la contraseña son obligatorios.";
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "El correo electrónico no está registrado.";
                return View();
            }

            var esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");

            // ── LoginSeguridad en lugar de LoginUsuarios ──
            var loginRecord = await _context.LoginSeguridad
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (loginRecord == null)
            {
                loginRecord = new LoginSeguridad
                {
                    UserId = user.Id,
                    IntentosFallidos = 0
                };
                _context.LoginSeguridad.Add(loginRecord);
                await _context.SaveChangesAsync();
            }

            // Verificar bloqueo
            if (!esAdmin && loginRecord.BloqueadoHasta.HasValue
                && loginRecord.BloqueadoHasta > DateTime.Now)
            {
                var restante = (loginRecord.BloqueadoHasta.Value - DateTime.Now).Minutes + 1;
                ViewBag.Error = $"Tu cuenta está bloqueada. Intenta de nuevo en {restante} minuto(s).";
                return View();
            }

            var passwordValida = await _userManager.CheckPasswordAsync(user, password);

            if (!passwordValida)
            {
                if (!esAdmin)
                {
                    loginRecord.IntentosFallidos++;
                    loginRecord.UltimoIntento = DateTime.Now;

                    var intentosRestantes = MaxIntentos - loginRecord.IntentosFallidos;

                    if (loginRecord.IntentosFallidos >= MaxIntentos)
                    {
                        loginRecord.BloqueadoHasta = DateTime.Now.AddMinutes(MinutosBloqueo);
                        loginRecord.IntentosFallidos = 0;
                        await _context.SaveChangesAsync();
                        ViewBag.Error = $"Has superado el límite de intentos. Tu cuenta ha sido bloqueada por {MinutosBloqueo} minutos.";
                        return View();
                    }

                    await _context.SaveChangesAsync();
                    ViewBag.Error = $"Contraseña incorrecta. Te quedan {intentosRestantes} intento(s) antes de ser bloqueado.";
                }
                else
                {
                    ViewBag.Error = "Contraseña incorrecta.";
                }
                return View();
            }

            // Login exitoso
            loginRecord.IntentosFallidos = 0;
            loginRecord.BloqueadoHasta = null;
            loginRecord.UltimoIntento = DateTime.Now;
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: recordarme);

            return esAdmin
                ? RedirectToAction("Index", "Admin")
                : Redirect("/Perfil");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}