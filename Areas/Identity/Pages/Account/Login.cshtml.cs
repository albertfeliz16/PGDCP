#nullable disable
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace PGDCP.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        // Inyectamos las dependencias necesarias para la gestión de estado de sesión y acceso a datos de usuario
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(SignInManager<IdentityUser> signInManager,
                          UserManager<IdentityUser> userManager,
                          ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El correo es obligatorio")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Recordarme")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Limpiamos la cookie externa para asegurar un flujo de autenticación limpio (estándar de seguridad)
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Recargamos los esquemas externos en caso de que el ModelState falle y debamos re-renderizar la página
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Intentamos la autenticación mediante el SignInManager (gestiona cookies y claims automáticamente)
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Autenticación exitosa para: {Email}", Input.Email);

                    // 1. OBTENEMOS EL USUARIO Y SUS ROLES
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    // 2. REGLAS DE REDIRECCIÓN (Prioridad Técnica)
                    if (roles.Contains("Administrador"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (roles.Contains("Perito"))
                    {
                        return RedirectToAction("Index", "Valoracions");
                    }
                    else if (roles.Contains("Restaurador"))
                    {
                        return RedirectToAction("Index", "Conservacions");
                    }

                    // 3. PARA TODOS LOS DEMÁS (Coleccionistas y otros)
                    // Borra la línea que decía "LocalRedirect(returnUrl)" y pon esta:
                    return RedirectToAction("Index", "Home");
                }

                // Manejo de estados de seguridad adicionales (2FA y Bloqueos)
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("La cuenta del usuario {Email} ha sido bloqueada temporalmente.", Input.Email);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    // Error genérico por seguridad (no revelar si el email o la contraseña es el dato erróneo)
                    ModelState.AddModelError(string.Empty, "Credenciales inválidas. Inténtelo de nuevo.");
                    return Page();
                }
            }

            // Si llegamos aquí, hubo un fallo en el modelo (ej: validación de campos)
            return Page();
        }
    }
}