#nullable disable

using Microsoft.AspNetCore.Identity;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP
{
    public static class SeedData
    {
        public static async Task SeedUsuarios(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            // ── Roles ──
            string[] roles = { "Administrador", "Coleccionista", "Restaurador", "Perito" };
            foreach (var rol in roles)
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));

            // ── Usuarios ──
            var usuarios = new[]
            {
                new { Email = "administrador@pgdcp.com", Password = "Administrador123", Rol = "Administrador",
                      Nombre = "Administrador", Apellido = "PGDCP", Sexo = "Masculino",
                      Fecha = new DateTime(2000, 2, 2), Telefono = "" },

                new { Email = "geisel@gmail.com",        Password = "Geisel123",        Rol = "Coleccionista",
                      Nombre = "Geisel", Apellido = "Ledesma", Sexo = "Masculino",
                      Fecha = new DateTime(2000, 10, 6), Telefono = "8098877777" },

                new { Email = "deivicontrol@gmail.com",  Password = "Deivicontrol123",  Rol = "Coleccionista",
                      Nombre = "albert", Apellido = "feliz", Sexo = "Masculino",
                      Fecha = new DateTime(2001, 5, 16), Telefono = "8098876666" },
            };

            foreach (var u in usuarios)
            {
                if (await userManager.FindByEmailAsync(u.Email) != null) continue;

                var user = new IdentityUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, u.Password);
                if (!result.Succeeded) continue;

                await userManager.AddToRoleAsync(user, u.Rol);

                var perfil = new PerfilUsuario
                {
                    UserId = user.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    FechaNacimiento = u.Fecha,
                    Sexo = u.Sexo,
                    Telefono = string.IsNullOrEmpty(u.Telefono) ? null : u.Telefono
                };
                context.PerfilesUsuario.Add(perfil);

                var login = new LoginSeguridad
                {
                    UserId = user.Id,
                    IntentosFallidos = 0
                };
                context.LoginSeguridad.Add(login);
            }

            await context.SaveChangesAsync();
        }
    }
}