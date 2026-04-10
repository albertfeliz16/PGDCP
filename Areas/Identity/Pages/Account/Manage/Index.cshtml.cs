#nullable disable
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Teléfono")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "El nombre es obligatorio")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El apellido es obligatorio")]
            public string Apellido { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime FechaNacimiento { get; set; }

            [Required] public string Sexo { get; set; }
        }

        // --- ESTE ES EL MÉTODO QUE TE FALTABA ---
        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            string userIdActual = user.Id; // QUITA EL ERROR DE ÁRBOL DINÁMICO

            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userIdActual);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Nombre = perfil?.Nombre ?? "",
                Apellido = perfil?.Apellido ?? "",
                FechaNacimiento = perfil?.FechaNacimiento ?? DateTime.Today.AddYears(-18),
                Sexo = perfil?.Sexo ?? ""
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound("Error al cargar usuario");
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            string userIdActual = user.Id; // QUITA EL ERROR DE ÁRBOL DINÁMICO

            var perfil = await _context.PerfilesUsuario
                .FirstOrDefaultAsync(p => p.UserId == userIdActual);

            if (perfil != null)
            {
                perfil.Nombre = Input.Nombre;
                perfil.Apellido = Input.Apellido;
                perfil.FechaNacimiento = Input.FechaNacimiento;
                perfil.Sexo = Input.Sexo;
                perfil.Telefono = Input.PhoneNumber;

                _context.PerfilesUsuario.Update(perfil);
                await _context.SaveChangesAsync();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Tu perfil ha sido actualizado con éxito en SQL Server";
            return RedirectToPage();
        }
    }
}