using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
                    .Include(o => o.Epoca)
                    .Include(o => o.Estilo)
                    .Include(o => o.Categoria)
                    .Include(o => o.Ubicacion)
                : _context.Obras
                    .Include(o => o.Epoca)
                    .Include(o => o.Estilo)
                    .Include(o => o.Categoria)
                    .Include(o => o.Ubicacion)
                    .Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(o =>
                    o.Titulo.Contains(buscar) ||
                    (o.Autor != null && o.Autor.Contains(buscar)) ||
                    (o.Epoca != null && o.Epoca.Nombre.Contains(buscar)) ||
                    (o.Categoria != null && o.Categoria.Nombre.Contains(buscar)));

            ViewBag.Buscar = buscar;
            return View(await query.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var obra = await _context.Obras
                .Include(o => o.Epoca)
                .Include(o => o.Estilo)
                .Include(o => o.Categoria)
                .Include(o => o.Ubicacion)
                .Include(o => o.Imagenes)
                .Include(o => o.ObraMateriales)!.ThenInclude(om => om.Material)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obra == null) return NotFound();
            return View(obra);
        }

        [Authorize(Roles = "Administrador,Coleccionista")]
        public IActionResult Create()
        {
            CargarCatalogos();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Create(
            [Bind("Titulo,Autor,CategoriaId,EpocaId,EstiloId,UbicacionId,Descripcion,FechaAdquisicion,ValorEstimado")] Obra obra,
            IFormFile? imagenArchivo)
        {
            if (ModelState.IsValid)
            {
                obra.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(obra);
                await _context.SaveChangesAsync();

                // Guardar imagen si se subió
                if (imagenArchivo != null && imagenArchivo.Length > 0)
                {
                    var urlImagen = await GuardarImagen(imagenArchivo);
                    if (urlImagen == null)
                    {
                        ModelState.AddModelError("", "Solo se permiten imágenes (jpg, png, gif, webp) de máximo 5MB.");
                        CargarCatalogos(obra);
                        return View(obra);
                    }
                    _context.ObraImagenes.Add(new ObraImagen
                    {
                        ObraId = obra.Id,
                        Url = urlImagen,
                        EsPrincipal = true
                    });
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = "Obra registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            CargarCatalogos(obra);
            return View(obra);
        }

        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var obra = await _context.Obras
                .Include(o => o.Imagenes)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (obra == null) return NotFound();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Administrador") && obra.UserId != userId) return Forbid();
            CargarCatalogos(obra);
            return View(obra);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Titulo,Autor,CategoriaId,EpocaId,EstiloId,UbicacionId,Descripcion,FechaAdquisicion,ValorEstimado")] Obra obra,
            IFormFile? imagenArchivo)
        {
            if (id != obra.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var obraOriginal = await _context.Obras.AsNoTracking()
                        .FirstOrDefaultAsync(o => o.Id == id);
                    obra.UserId = obraOriginal?.UserId;

                    _context.Update(obra);
                    await _context.SaveChangesAsync();

                    // Guardar nueva imagen si se subió
                    if (imagenArchivo != null && imagenArchivo.Length > 0)
                    {
                        var urlImagen = await GuardarImagen(imagenArchivo);
                        if (urlImagen == null)
                        {
                            ModelState.AddModelError("", "Solo se permiten imágenes (jpg, png, gif, webp) de máximo 5MB.");
                            CargarCatalogos(obra);
                            return View(obra);
                        }
                        // Marcar las anteriores como no principales
                        var imagenesAnteriores = await _context.ObraImagenes
                            .Where(i => i.ObraId == id).ToListAsync();
                        foreach (var img in imagenesAnteriores)
                            img.EsPrincipal = false;

                        _context.ObraImagenes.Add(new ObraImagen
                        {
                            ObraId = obra.Id,
                            Url = urlImagen,
                            EsPrincipal = true
                        });
                        await _context.SaveChangesAsync();
                    }

                    TempData["Success"] = "Obra actualizada correctamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Obras.Any(e => e.Id == obra.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            CargarCatalogos(obra);
            return View(obra);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var obra = await _context.Obras
                .Include(o => o.Epoca)
                .Include(o => o.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);
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

        // ── Helpers privados ──

        private void CargarCatalogos(Obra? obra = null)
        {
            ViewBag.CategoriaId = new SelectList(_context.Categorias.OrderBy(c => c.Nombre), "Id", "Nombre", obra?.CategoriaId);
            ViewBag.EpocaId = new SelectList(_context.Epocas.OrderBy(e => e.Nombre), "Id", "Nombre", obra?.EpocaId);
            ViewBag.EstiloId = new SelectList(_context.Estilos.OrderBy(e => e.Nombre), "Id", "Nombre", obra?.EstiloId);
            ViewBag.UbicacionId = new SelectList(_context.Ubicaciones.OrderBy(u => u.Nombre), "Id", "Nombre", obra?.UbicacionId);
            ViewBag.Materiales = _context.Materiales.OrderBy(m => m.Nombre).ToList();
        }

        private async Task<string?> GuardarImagen(IFormFile archivo)
        {
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(archivo.FileName).ToLower();

            if (!extensionesPermitidas.Contains(extension)) return null;
            if (archivo.Length > 5 * 1024 * 1024) return null;

            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "obras");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid().ToString() + extension;
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using var stream = new FileStream(rutaCompleta, FileMode.Create);
            await archivo.CopyToAsync(stream);

            return "/images/obras/" + nombreArchivo;
        }
    }
}