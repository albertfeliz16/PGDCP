using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize]
    public class ObrasController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ObrasController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? buscar)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrador");
            var isRestaurador = User.IsInRole("Restaurador");
            var isPerito = User.IsInRole("Perito");

            IQueryable<Obra> query = (isAdmin || isRestaurador || isPerito)
                ? _context.Obras
                : _context.Obras.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(o =>
                    o.Titulo.Contains(buscar) ||
                    (o.Autor != null && o.Autor.Contains(buscar)) ||
                    (o.Epoca != null && o.Epoca.Contains(buscar)));

            ViewBag.Buscar = buscar;
            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var obra = await _context.Obras.FirstOrDefaultAsync(m => m.Id == id);
            if (obra == null) return NotFound();
            return View(obra);
        }

        [Authorize(Roles = "Administrador,Coleccionista")]
        public IActionResult Create() => View();

        // ── CREATE POST ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Create(
            [Bind("Titulo,Autor,Epoca,Estilo,Material,FechaAdquisicion,ValorEstimado,ImagenUrl")] Obra obra,
            IFormFile? imagenArchivo)
        {
            if (ModelState.IsValid)
            {
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    try
                    {
                        // Validar que sea una imagen
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var extension = Path.GetExtension(imagenArchivo.FileName).ToLower();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("", "Solo se permiten imágenes (jpg, png, gif, webp).");
                            return View(obra);
                        }

                        // Validar tamaño máximo 5MB
                        if (imagenArchivo.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "La imagen no puede superar 5MB.");
                            return View(obra);
                        }

                        var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "obras");
                        Directory.CreateDirectory(carpeta);

                        var nombreArchivo = Guid.NewGuid().ToString() + extension;
                        var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                            await imagenArchivo.CopyToAsync(stream);

                        obra.ImagenUrl = "/images/obras/" + nombreArchivo;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error al subir la imagen: {ex.Message}");
                        return View(obra);
                    }
                }

                obra.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(obra);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Obra registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(obra);
        }

        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var obra = await _context.Obras.FindAsync(id);
            if (obra == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Administrador") && obra.UserId != userId) return Forbid();
            return View(obra);
        }

        // ── EDIT POST ──
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Titulo,Autor,Epoca,Estilo,Material,FechaAdquisicion,ValorEstimado,ImagenUrl")] Obra obra,
            IFormFile? imagenArchivo)
        {
            if (id != obra.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    try
                    {
                        // Validar que sea una imagen
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var extension = Path.GetExtension(imagenArchivo.FileName).ToLower();

                        if (!extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("", "Solo se permiten imágenes (jpg, png, gif, webp).");
                            return View(obra);
                        }

                        // Validar tamaño máximo 5MB
                        if (imagenArchivo.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("", "La imagen no puede superar 5MB.");
                            return View(obra);
                        }

                        var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "obras");
                        Directory.CreateDirectory(carpeta);

                        var nombreArchivo = Guid.NewGuid().ToString() + extension;
                        var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                            await imagenArchivo.CopyToAsync(stream);

                        obra.ImagenUrl = "/images/obras/" + nombreArchivo;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error al subir la imagen: {ex.Message}");
                        return View(obra);
                    }
                }

                try
                {
                    // Mantener el UserId original
                    var obraOriginal = await _context.Obras.AsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id == id);
                    obra.UserId = obraOriginal?.UserId;

                    _context.Update(obra);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Obra actualizada correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Obras.Any(e => e.Id == obra.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(obra);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var obra = await _context.Obras.FirstOrDefaultAsync(m => m.Id == id);
            if (obra == null) return NotFound();
            return View(obra);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obra = await _context.Obras.FindAsync(id);
            if (obra != null) _context.Obras.Remove(obra);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Obra eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}