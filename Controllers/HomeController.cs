using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PGDCP.Data;
using PGDCP.Models;

namespace PGDCP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId  = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Administrador");

                ViewBag.TotalObras = isAdmin
                    ? await _context.Obras.CountAsync()
                    : await _context.Obras.Where(o => o.UserId == userId).CountAsync();

                ViewBag.TotalConservaciones = await _context.Conservaciones.CountAsync();
                ViewBag.TotalValoraciones   = await _context.Valoraciones.CountAsync();

                ViewBag.ValorTotal = isAdmin
                    ? await _context.Obras.SumAsync(o => (decimal?)o.ValorEstimado) ?? 0
                    : await _context.Obras.Where(o => o.UserId == userId).SumAsync(o => (decimal?)o.ValorEstimado) ?? 0;

                ViewBag.ObrasRecientes = isAdmin
                    ? await _context.Obras.OrderByDescending(o => o.FechaAdquisicion).Take(5).ToListAsync()
                    : await _context.Obras.Where(o => o.UserId == userId).OrderByDescending(o => o.FechaAdquisicion).Take(5).ToListAsync();

                // Para restaurador: últimas conservaciones
                if (User.IsInRole("Restaurador"))
                    ViewBag.UltimasConservaciones = await _context.Conservaciones
                        .Include(c => c.Obra).OrderByDescending(c => c.FechaIntervencion).Take(5).ToListAsync();

                // Para perito: últimas valoraciones
                if (User.IsInRole("Perito"))
                    ViewBag.UltimasValoraciones = await _context.Valoraciones
                        .Include(v => v.Obra).OrderByDescending(v => v.FechaValoracion).Take(5).ToListAsync();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
