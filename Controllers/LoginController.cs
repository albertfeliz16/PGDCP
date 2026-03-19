#nullable disable
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;
using System.Security.Claims;

namespace PGDCP.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        // Máximo de intentos antes de bloquear
        private const int MaxIntentos = 5;
        // Minutos de bloqueo
        private const int MinutosBloqueo = 15;

        public LoginController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: mostrar formulario
        [HttpGet]
        public IActionResult Index()
        {
            // Si ya está autenticado, redirigir
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Perfil");

            return View();
        }

        // POST: procesar login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string email, string password, bool recordarme = false)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "El correo y la contraseña son obligatorios.";
                return View();
            }

            // Buscar el usuario en Identity
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Error = "El correo electrónico no está registrado.";
                return View();
            }

            // Verificar si es administrador (nunca se bloquea)
            var esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");

            // Buscar o crear registro de login
            var loginRecord = await _context.LoginUsuarios
                .FirstOrDefaultAsync(l => l.UserId == user.Id);

            if (loginRecord == null)
            {
                loginRecord = new LoginUsuario
                {
                    UserId = user.Id,
                    Email = email,
                    IntentosFallidos = 0
                };
                _context.LoginUsuarios.Add(loginRecord);
                await _context.SaveChangesAsync();
            }

            // Verificar bloqueo (solo si no es admin)
            if (!esAdmin && loginRecord.BloqueadoHasta.HasValue
                && loginRecord.BloqueadoHasta > DateTime.Now)
            {
                var restante = (loginRecord.BloqueadoHasta.Value - DateTime.Now).Minutes + 1;
                ViewBag.Error = $"Tu cuenta está bloqueada. Intenta de nuevo en {restante} minuto(s).";
                return View();
            }

            // Verificar contraseña
            var passwordValida = await _userManager.CheckPasswordAsync(user, password);

            if (!passwordValida)
            {
                // Incrementar intentos fallidos (solo si no es admin)
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

            // Login exitoso — resetear intentos
            loginRecord.IntentosFallidos = 0;
            loginRecord.BloqueadoHasta = null;
            loginRecord.UltimoIntento = DateTime.Now;
            await _context.SaveChangesAsync();

            // Iniciar sesión con Identity
            await _signInManager.SignInAsync(user, isPersistent: recordarme);

            // Redirigir según rol
            if (esAdmin)
                return RedirectToAction("Index", "Admin");

            return Redirect("/Perfil");
        }

        // POST: cerrar sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Salir()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}