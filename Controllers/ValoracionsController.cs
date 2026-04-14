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
                query = query.Where(v =>
                    (v.Obra != null && v.Obra.Titulo.Contains(buscar)) ||
                    (v.Observaciones != null && v.Observaciones.Contains(buscar)) ||
                    v.MetodoValoracion.Contains(buscar) ||
                    v.EstadoAutenticidad.Contains(buscar));

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
            ViewData["ObraId"] = new SelectList(_context.Obras.OrderBy(o => o.Titulo), "Id", "Titulo");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ObraId,ValorEstimado,Observaciones,MetodoValoracion,EstadoAutenticidad,FactoresAjuste")] Valoracion valoracion)
        {
            if (ModelState.IsValid)
            {
                // Fecha y Perito automáticos
                valoracion.FechaValoracion = DateTime.Today;
                valoracion.PeritoId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _context.Add(valoracion);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Dictamen pericial guardado correctamente.";
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
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,ObraId,ValorEstimado,Observaciones,PeritoId,FechaValoracion,MetodoValoracion,EstadoAutenticidad,FactoresAjuste")] Valoracion valoracion)
        {
            if (id != valoracion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(valoracion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Valoraciones.Any(e => e.Id == valoracion.Id)) return NotFound();
                    else throw;
                }
                TempData["Success"] = "Valoración pericial actualizada.";
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

            TempData["Success"] = "El registro ha sido eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}