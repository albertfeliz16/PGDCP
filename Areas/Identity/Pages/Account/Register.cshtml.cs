#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context; // Para guardar el perfil

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            ApplicationDbContext context) // Inyectamos el DbContext
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            // Correo electrónico
            [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
            [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; }

            // Nombre
            [Required(ErrorMessage = "El nombre es obligatorio.")]
            [StringLength(50)]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            // Apellido
            [Required(ErrorMessage = "El apellido es obligatorio.")]
            [StringLength(50)]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            // Fecha de nacimiento
            [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
            [DataType(DataType.Date)]
            [MayorDeEdad]
            [Display(Name = "Fecha de nacimiento")]
            public DateTime FechaNacimiento { get; set; }

            // Sexo
            [Required(ErrorMessage = "El sexo es obligatorio.")]
            [Display(Name = "Sexo")]
            public string Sexo { get; set; }

            // Teléfono
            [Phone(ErrorMessage = "Formato de teléfono no válido.")]
            [Display(Name = "Teléfono")]
            public string Telefono { get; set; }

            // Rol
            [Required(ErrorMessage = "El rol es obligatorio.")]
            [Display(Name = "Rol")]
            public string Rol { get; set; }

            // Contraseña
            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            // Confirmar contraseña
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
            public string ConfirmPassword { get; set; }
        }

        // Cargar la página de registro
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        // Procesar el formulario de registro
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Crear el usuario de Identity (solo email y contraseña)
                var user = CreateUser();
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Se registró un nuevo usuario.");

                    // Guardar el perfil extra en la tabla PerfilesUsuario
                    var perfil = new PerfilUsuario
                    {
                        UserId = await _userManager.GetUserIdAsync(user),
                        Nombre = Input.Nombre,
                        Apellido = Input.Apellido,
                        FechaNacimiento = Input.FechaNacimiento,
                        Sexo = Input.Sexo,
                        Telefono = Input.Telefono,
                        Rol = Input.Rol
                    };
                    _context.PerfilesUsuario.Add(perfil);
                    await _context.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(user, Input.Rol);

                    // Generar confirmación de correo
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirma tu correo",
                        $"Confirma tu cuenta haciendo <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clic aquí</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // Iniciar sesión automáticamente
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Redirect("/Perfil");
                    }
                }

                // Mostrar errores de Identity
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"No se puede crear una instancia de '{nameof(IdentityUser)}'.");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Se requiere un store con soporte de correo electrónico.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}