#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CatalogosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CatalogosController(ApplicationDbContext context) => _context = context;

        // ── CATEGORÍAS ──

        public async Task<IActionResult> Categorias() =>
            View("Categorias", await _context.Categorias.OrderBy(c => c.Nombre).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(string nombre, string descripcion)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.Categorias.Add(new Categoria { Nombre = nombre, Descripcion = descripcion });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Categoría creada.";
            }
            return RedirectToAction(nameof(Categorias));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var item = await _context.Categorias.FindAsync(id);
            if (item != null) { _context.Categorias.Remove(item); await _context.SaveChangesAsync(); TempData["Success"] = "Categoría eliminada."; }
            return RedirectToAction(nameof(Categorias));
        }

        // ── ÉPOCAS ──

        public async Task<IActionResult> Epocas() =>
            View("Epocas", await _context.Epocas.OrderBy(e => e.Nombre).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEpoca(string nombre, string descripcion, short? sigloDesde, short? sigloHasta)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.Epocas.Add(new Epoca { Nombre = nombre, Descripcion = descripcion, SigloDesde = sigloDesde, SigloHasta = sigloHasta });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Época creada.";
            }
            return RedirectToAction(nameof(Epocas));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarEpoca(int id)
        {
            var item = await _context.Epocas.FindAsync(id);
            if (item != null) { _context.Epocas.Remove(item); await _context.SaveChangesAsync(); TempData["Success"] = "Época eliminada."; }
            return RedirectToAction(nameof(Epocas));
        }

        // ── ESTILOS ──

        public async Task<IActionResult> Estilos() =>
            View("Estilos", await _context.Estilos.OrderBy(e => e.Nombre).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearEstilo(string nombre, string descripcion)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.Estilos.Add(new Estilo { Nombre = nombre, Descripcion = descripcion });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Estilo creado.";
            }
            return RedirectToAction(nameof(Estilos));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarEstilo(int id)
        {
            var item = await _context.Estilos.FindAsync(id);
            if (item != null) { _context.Estilos.Remove(item); await _context.SaveChangesAsync(); TempData["Success"] = "Estilo eliminado."; }
            return RedirectToAction(nameof(Estilos));
        }

        // ── UBICACIONES ──

        public async Task<IActionResult> Ubicaciones() =>
            View("Ubicaciones", await _context.Ubicaciones.OrderBy(u => u.Nombre).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUbicacion(string nombre, string descripcion, string direccion)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.Ubicaciones.Add(new Ubicacion { Nombre = nombre, Descripcion = descripcion, Direccion = direccion });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ubicación creada.";
            }
            return RedirectToAction(nameof(Ubicaciones));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUbicacion(int id)
        {
            var item = await _context.Ubicaciones.FindAsync(id);
            if (item != null) { _context.Ubicaciones.Remove(item); await _context.SaveChangesAsync(); TempData["Success"] = "Ubicación eliminada."; }
            return RedirectToAction(nameof(Ubicaciones));
        }

        // ── MATERIALES ──

        public async Task<IActionResult> Materiales() =>
            View("Materiales", await _context.Materiales.OrderBy(m => m.Nombre).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearMaterial(string nombre, string descripcion)
        {
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                _context.Materiales.Add(new Material { Nombre = nombre, Descripcion = descripcion });
                await _context.SaveChangesAsync();
                TempData["Success"] = "Material creado.";
            }
            return RedirectToAction(nameof(Materiales));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarMaterial(int id)
        {
            var item = await _context.Materiales.FindAsync(id);
            if (item != null) { _context.Materiales.Remove(item); await _context.SaveChangesAsync(); TempData["Success"] = "Material eliminado."; }
            return RedirectToAction(nameof(Materiales));
        }
    }
}