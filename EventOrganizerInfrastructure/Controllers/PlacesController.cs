﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventOrganizerDomain.Model;
using EventOrganizerInfrastructure;

namespace EventOrganizerInfrastructure.Controllers
{
    public class PlacesController : Controller
    {
        private readonly DbeventOrganizerContext _context;

        public PlacesController(DbeventOrganizerContext context)
        {
            _context = context;
        }

        // GET: Places
        public async Task<IActionResult> Index(int? id, string? name, string? country)
        {
            if (id == null) return RedirectToAction("Cities", "Index");

            ViewBag.CityId = id;
            ViewBag.CountryId = country;
            ViewBag.CityName = name;
            var placeByCity = _context.Places.Where(p => p.CityId == id).Include(p => p.City).Include(p => p.PlaceType);
            return View(await placeByCity.ToListAsync());
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
                .Include(p => p.PlaceType)
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
