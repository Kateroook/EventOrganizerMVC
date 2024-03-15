using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventOrganizerDomain.Model;
using EventOrganizerInfrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using NuGet.Packaging;

namespace EventOrganizerInfrastructure.Controllers
{
    public class EventsController : Controller
    {
        private readonly DbeventOrganizerContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EventsController(DbeventOrganizerContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {

            var dbeventOrganizerContext = _context.Events
    .Include(e => e.Place).ThenInclude(p => p.City)
    .Include(e => e.Organizers)
    .Include(e => e.Tags)
    .OrderByDescending(e => e.DateTimeStart);
            return View(await dbeventOrganizerContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
                .Include(e => e.Organizers)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name");
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Title");
            ViewData["OrganizerId"] = new SelectList(_context.Users.Where(u => u.Role.Name.ToLower() == "organizer"), "Id", "OrganizationOrFullName");

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaceId,Title,Description,Speaker,DateTimeStart,DateTimeEnd,Price,Capacity,PictureUrl,Id")] Event @event, int[] tags, int[] organizers)
        {
            Place place = _context.Places.Include(pt => pt.PlaceType).Include(c => c.City).FirstOrDefault(p => p.Id == @event.PlaceId);
            @event.Place = place;
            ModelState.Clear();
            TryValidateModel(place);

            if (ModelState.IsValid)
            {
                _context.Add(@event);

                foreach (var tagId in tags)
                {
                    var eventTag = _context.Tags.Find(tagId);
                    if (eventTag != null)
                        @event.Tags.Add(eventTag);
                }

                foreach (var organizerId in organizers)
                {
                    var organizer = _context.Users.Find(organizerId);
                    if (organizer != null)
                        @event.Organizers.Add(organizer);
                }

                //if((place.Capacity != null && @event.Capacity == null) || (place.Capacity != null && @event.Capacity >= place.Capacity))
                //{
                //    @event.Capacity = place.Capacity;
                //}


                @event.CreatedAt = DateTime.Now;
                @event.LastUpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name", @event.PlaceId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Title", @event.Tags);
            ViewData["OrganizerId"] = new SelectList(_context.Users.Where(u => u.Role.Name.ToLower() == "organizer"), "Id", "OrganizationOrFullName", @event.Organizers);

            return View(@event);
        }


        // GET: Events/Edit/5

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
                .Include(e => e.Organizers)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name", @event.PlaceId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Title", @event.Tags);
            ViewData["OrganizerId"] = new SelectList(_context.Users.Where(u => u.Role.Name.ToLower() == "organizer"), "Id", "OrganizationOrFullName", @event.Organizers);

            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlaceId,Title,Description,Speaker,DateTimeStart,DateTimeEnd,Price,Capacity,PictureUrl")] Event @event, int[] tags, int[] organizers)
        {
            Place place = _context.Places.Include(pt => pt.PlaceType).Include(c => c.City).ThenInclude(c => c.Country).FirstOrDefault(p => p.Id == @event.PlaceId);
            @event.Place = place;
            ModelState.Clear();
            TryValidateModel(place);

            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events
                        .Include(e => e.Tags)
                        .Include(e => e.Organizers)
                        .FirstOrDefaultAsync(e => e.Id == id);

                    if (existingEvent == null)
                    {
                        return NotFound();
                    }

                    existingEvent.PlaceId = @event.PlaceId;
                    existingEvent.Title = @event.Title;
                    existingEvent.Description = @event.Description;
                    existingEvent.Speaker = @event.Speaker;
                    existingEvent.DateTimeStart = @event.DateTimeStart;
                    existingEvent.DateTimeEnd = @event.DateTimeEnd;
                    existingEvent.Price = @event.Price;
                    existingEvent.Capacity = @event.Capacity;
                    existingEvent.PictureUrl = @event.PictureUrl;

                    existingEvent.LastUpdatedAt = DateTime.Now;

                    // Remove existing tags and add new ones
                    existingEvent.Tags.Clear();
                    foreach (var tagId in tags)
                    {
                        var tag = await _context.Tags.FindAsync(tagId);
                        if (tag != null)
                        {
                            existingEvent.Tags.Add(tag);
                        }
                    }

                    // Remove existing organizers and add new ones
                    existingEvent.Organizers.Clear();
                    foreach (var organizerId in organizers)
                    {
                        var organizer = await _context.Users.FindAsync(organizerId);
                        if (organizer != null)
                        {
                            existingEvent.Organizers.Add(organizer);
                        }
                    }

                    _context.Update(existingEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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

            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name", @event.PlaceId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Title", @event.Tags);
            ViewData["OrganizerId"] = new SelectList(_context.Users.Where(u => u.Role.Name.ToLower() == "organizer"), "Id", "OrganizationOrFullName", @event.Organizers);

            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
                .Include(e => e.Tags)
                .Include(e => e.Organizers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

    }
}
