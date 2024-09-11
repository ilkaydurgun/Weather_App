using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Weather_App.Context;
using Weather_App.Models;

namespace Weather_App.Controllers
{
    public class WeathersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeathersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Weathers
        public async Task<IActionResult> Index()
        {
            return View(await _context.WeatherApps.ToListAsync());
        }

        // GET: Weathers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weather = await _context.WeatherApps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weather == null)
            {
                return NotFound();
            }

            return View(weather);
        }

        // GET: Weathers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Weathers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,City,Date,Temperature,State")] Weather weather)
        {
            if (ModelState.IsValid)
            {
                _context.Add(weather);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weather);
        }

        // GET: Weathers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weather = await _context.WeatherApps.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }
            return View(weather);
        }

        // POST: Weathers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,City,Date,Temperature,State")] Weather weather)
        {
            if (id != weather.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weather);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeatherExists(weather.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(weather);
        }
        // GET: Weathers/Filter
        public IActionResult Filter()
        {
            return View();
        }

        // POST: Weathers/Filter
        [HttpPost]
        public async Task<IActionResult> Filter(string city, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrEmpty(city) || startDate == default || endDate == default)
            {
                return BadRequest("Please provide valid city name and dates.");
            }

            var weatherData = await _context.WeatherApps
                .Where(w => w.City == city && w.Date >= startDate && w.Date <= endDate)
                .ToListAsync();

            if (weatherData == null || weatherData.Count == 0)
            {
                return NotFound($"No weather data found for {city} between {startDate.ToShortDateString()} and {endDate.ToShortDateString()}.");
            }

            return View("WeatherResults", weatherData);  // This will redirect to a view that displays the results
        }

        // GET: Weathers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weather = await _context.WeatherApps
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weather == null)
            {
                return NotFound();
            }

            return View(weather);
        }

        // POST: Weathers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var weather = await _context.WeatherApps.FindAsync(id);
            if (weather != null)
            {
                _context.WeatherApps.Remove(weather);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeatherExists(int id)
        {
            return _context.WeatherApps.Any(e => e.Id == id);
        }
    }
}
