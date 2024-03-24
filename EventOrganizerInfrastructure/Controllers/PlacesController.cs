using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventOrganizerDomain.Model;
using EventOrganizerInfrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventOrganizerInfrastructure.Controllers
{
    public class PlacesController : Controller
    {
        private readonly DbeventOrganizerContext _context;
        private readonly JsonSerializerOptions _jsonOptions;
        public PlacesController(DbeventOrganizerContext context)
        {
            _context = context;
            _jsonOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _context.Countries.ToListAsync();
            return Json(countries);
        }
        

        [HttpGet]
        public async Task<IActionResult> GetCitiesInCountry(int countryId)
        {
            var cities = await _context.Cities.Where(c => c.CountryId == countryId).ToListAsync();
            return Json(cities);
        }
        
        public IActionResult PlacesInCity(int cityId)
        {
            var places = _context.Places.Where(p => p.CityId == cityId).ToList();
            return PartialView("_PlaceList", places);
        }        
        public IActionResult PlacesOnline()
        {
            var places = _context.Places.Where(p => p.PlaceType.Name.ToLower() == "online").ToList();
            return PartialView("_PlaceList", places);
        }

        //// Действие для загрузки списка мест по идентификатору города
        //[HttpGet]
        //public async Task<IActionResult> GetPlacesInCity(int cityId)
        //{
        //    try
        //    {
        //        var places = await _context.Places
        //            .Where(p => p.CityId == cityId)
        //            .ToListAsync();

        //        return PartialView("_PlaceList", places);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false,error = ex.Message });
        //    }
        //}

        // GET: Places
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return RedirectToAction("Cities", "Index");

            ViewBag.CityId = id;

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            ViewBag.CityName = city.Name;

            var popularCities = await _context.Cities.OrderByDescending(c => c.Places.Count).Take(5).ToListAsync();
            ViewBag.PopularCities = popularCities;


            var popularEventsInCity = await _context.Events
                .Where(e => e.Place.CityId == id)
                .OrderByDescending(e => e.Registrations.Count)
                .Take(5)
                .ToListAsync();

            ViewBag.PopularEventsInCity = popularEventsInCity;

            var placesInCity = await _context.Places
                .Where(p => p.CityId == id)
                .Include(p => p.City)
                    .ThenInclude(p => p.Country)
                .Include(p => p.PlaceType)
                .Include(p => p.Events)
                .ToListAsync();

            return View(placesInCity);
        }


        // GET: Places/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.City)
                .ThenInclude(p => p.Country)
                .Include(p => p.PlaceType)
                .Include(p => p.Events)
                    .ThenInclude(o => o.Organizers)
                .Include(o => o.Events)
                    .ThenInclude(t => t.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // GET: Places/Create
        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name");
            ViewData["PlaceTypeId"] = new SelectList(_context.PlaceTypes, "Id", "Name");
            return View();
        }

        // POST: Places/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaceTypeId,CityId,Name,Description,UnitNumber,AddressLine1,AddressLine2,Zip,CoordinatesCol1,CoordinatesCol2,PlaceImage,Capacity,PhoneNumber,ContactEmail,Website,Id")] Place place)
        {
            if (ModelState.IsValid)
            {
                _context.Add(place);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", place.CityId);
            ViewData["PlaceTypeId"] = new SelectList(_context.PlaceTypes, "Id", "Name", place.PlaceTypeId);
            return View(place);
        }

        // GET: Places/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", place.CityId);
            ViewData["PlaceTypeId"] = new SelectList(_context.PlaceTypes, "Id", "Name", place.PlaceTypeId);
            return View(place);
        }

        // POST: Places/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaceTypeId,CityId,Name,Description,UnitNumber,AddressLine1,AddressLine2,Zip,CoordinatesCol1,CoordinatesCol2,PlaceImage,Capacity,PhoneNumber,ContactEmail,Website,Id")] Place place)
        {
            if (id != place.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(place);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaceExists(place.Id))
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
            ViewData["CityId"] = new SelectList(_context.Cities, "Id", "Name", place.CityId);
            ViewData["PlaceTypeId"] = new SelectList(_context.PlaceTypes, "Id", "Name", place.PlaceTypeId);
            return View(place);
        }

        // GET: Places/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var place = await _context.Places
                .Include(p => p.City)
                .Include(p => p.PlaceType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (place == null)
            {
                return NotFound();
            }

            return View(place);
        }

        // POST: Places/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place != null)
            {
                _context.Places.Remove(place);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaceExists(int id)
        {
            return _context.Places.Any(e => e.Id == id);
        }
    }
}
