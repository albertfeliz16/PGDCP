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
            var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Administrador");
            var isRestaurador = User.IsInRole("Restaurador");
            var isPerito = User.IsInRole("Perito");

            // Restauradores y Peritos ven todas las obras (solo lectura para Restaurador)
            IQueryable<Obra> query = (isAdmin || isRestaurador || isPerito)
                ? _context.Obras
                : _context.Obras.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(buscar))
                query = query.Where(o => o.Titulo.Contains(buscar) || (o.Autor != null && o.Autor.Contains(buscar)) || (o.Epoca != null && o.Epoca.Contains(buscar)));

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

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Create([Bind("Titulo,Autor,Epoca,Estilo,Material,FechaAdquisicion,ValorEstimado,ImagenUrl")] Obra obra)
        {
            if (ModelState.IsValid)
            {
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

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Coleccionista")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Autor,Epoca,Estilo,Material,FechaAdquisicion,ValorEstimado,ImagenUrl,UserId")] Obra obra)
        {
            if (id != obra.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(obra); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!_context.Obras.Any(e => e.Id == obra.Id)) return NotFound(); else throw; }
                TempData["Success"] = "Obra actualizada correctamente.";
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
