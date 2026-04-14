using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize(Roles = "Administrador,Restaurador")]
    public class ConservacionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ConservacionsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? buscar)
        {
            var query = _context.Conservaciones.Include(c => c.Obra).AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(c =>
                    c.Diagnostico.Contains(buscar) ||
                    (c.Tratamiento != null && c.Tratamiento.Contains(buscar)) ||
                    c.EstadoConservacion.Contains(buscar));

            ViewBag.Buscar = buscar;
            return View(await query.OrderByDescending(c => c.FechaIntervencion).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var c = await _context.Conservaciones.Include(x => x.Obra).FirstOrDefaultAsync(m => m.Id == id);
            if (c == null) return NotFound();
            return View(c);
        }

        public IActionResult Create()
        {
            ViewData["ObraId"] = new SelectList(_context.Obras.OrderBy(o => o.Titulo), "Id", "Titulo");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ObraId,Diagnostico,Tratamiento,EstadoConservacion,TipoIntervencion,CostoIntervencion")] Conservacion conservacion)
        {
            if (ModelState.IsValid)
            {
                // Fecha de hoy automáticamente
                conservacion.FechaIntervencion = DateTime.Today;

                // El Restaurador es el usuario logueado actualmente
                conservacion.RestauradorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(conservacion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Registro de conservación guardado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo", conservacion.ObraId);
            return View(conservacion);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var c = await _context.Conservaciones.FindAsync(id);
            if (c == null) return NotFound();

            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo", c.ObraId);
            return View(c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,ObraId,Diagnostico,Tratamiento,FechaIntervencion,RestauradorId,EstadoConservacion,TipoIntervencion,CostoIntervencion")] Conservacion conservacion)
        {
            if (id != conservacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(conservacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Conservaciones.Any(e => e.Id == conservacion.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Registro de conservación actualizado.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo", conservacion.ObraId);
            return View(conservacion);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var c = await _context.Conservaciones.Include(x => x.Obra).FirstOrDefaultAsync(m => m.Id == id);
            if (c == null) return NotFound();
            return View(c);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var c = await _context.Conservaciones.FindAsync(id);
            if (c != null) _context.Conservaciones.Remove(c);
            await _context.SaveChangesAsync();

            TempData["Success"] = "El registro ha sido eliminado permanentemente.";
            return RedirectToAction(nameof(Index));
        }
    }
}