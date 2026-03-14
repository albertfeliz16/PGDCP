using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    [Authorize(Roles = "Administrador,Perito")]
    public class ValoracionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ValoracionsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index(string? buscar)
        {
            var query = _context.Valoraciones.Include(v => v.Obra).AsQueryable();
            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(v => (v.Obra != null && v.Obra.Titulo.Contains(buscar)) || (v.Observaciones != null && v.Observaciones.Contains(buscar)));
            ViewBag.Buscar = buscar;
            return View(await query.OrderByDescending(v => v.FechaValoracion).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var v = await _context.Valoraciones.Include(x => x.Obra).FirstOrDefaultAsync(m => m.Id == id);
            if (v == null) return NotFound();
            return View(v);
        }

        public IActionResult Create()
        {
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObraId,ValorEstimado,FechaValoracion,Observaciones")] Valoracion valoracion)
        {
            if (ModelState.IsValid)
            {
                valoracion.PeritoId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.Add(valoracion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Valoración registrada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo", valoracion.ObraId);
            return View(valoracion);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var v = await _context.Valoraciones.FindAsync(id);
            if (v == null) return NotFound();
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo", v.ObraId);
            return View(v);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ObraId,ValorEstimado,FechaValoracion,Observaciones,PeritoId")] Valoracion valoracion)
        {
            if (id != valoracion.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(valoracion); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!_context.Valoraciones.Any(e => e.Id == valoracion.Id)) return NotFound(); else throw; }
                TempData["Success"] = "Valoración actualizada.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ObraId"] = new SelectList(_context.Obras, "Id", "Titulo", valoracion.ObraId);
            return View(valoracion);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var v = await _context.Valoraciones.Include(x => x.Obra).FirstOrDefaultAsync(m => m.Id == id);
            if (v == null) return NotFound();
            return View(v);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var v = await _context.Valoraciones.FindAsync(id);
            if (v != null) _context.Valoraciones.Remove(v);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Valoración eliminada.";
            return RedirectToAction(nameof(Index));
        }
    }
}
