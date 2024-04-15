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
using System.Drawing.Printing;
using Microsoft.AspNetCore.Identity;
using EventOrganizerInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace EventOrganizerInfrastructure.Controllers
{
    public class EventsController : Controller
    {
        private readonly DbeventOrganizerContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        private const int PageSize = 12;
        public EventsController(DbeventOrganizerContext context, IWebHostEnvironment webHostEnvironment, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Events
        public async Task<IActionResult> Index(int? page)
        {
            int pageNumber = page ?? 1;
            var events = await _context.Events
                .Include(e => e.Place).ThenInclude(p => p.City)
                .OrderByDescending(e => e.DateTimeStart)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PageIndex = pageNumber;

            ViewBag.HasPreviousPage = pageNumber > 1;
            ViewBag.HasNextPage = events.Count == PageSize;

            int totalEvents = await _context.Events.CountAsync();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEvents / PageSize);

            return View(events);
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
                .ThenInclude(p => p.City)
                .ThenInclude(c => c.Country)
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
        [Authorize(Roles = "Organizer, Moderator")]
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name");
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Title");

            // Получаем текущего пользователя
            var currentUser = await _userManager.GetUserAsync(User);

            // Если текущий пользователь является организатором, скрываем поле выбора организаторов
            if (User.IsInRole("Organizer"))
            {
                ViewData["OrganizerId"] = new List<SelectListItem>
                {
                    new SelectListItem { Value = currentUser.Id.ToString(), Text = currentUser.OrganizationOrFullName }
                };
            }
            else
            {
                var organizers = await _userManager.GetUsersInRoleAsync("Organizer");
                // Формируем список организаторов для передачи в представление
                var organizerList = organizers.Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.OrganizationOrFullName // Или любое другое поле, которое вы хотите отобразить
                }).ToList();

                // Передаем список организаторов в представление
                ViewData["OrganizerId"] = organizerList;
            }

            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Organizer, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaceId,Title,Description,Speaker,DateTimeStart,DateTimeEnd,Price,Capacity,PictureUrl,Id")] Event @event, int[] tags, int[] organizers)
        {
            Place place = _context.Places.Include(pt => pt.PlaceType).Include(c => c.City).ThenInclude(c => c.Country).FirstOrDefault(p => p.Id == @event.PlaceId);
            @event.Place = place;
            ModelState.Clear();
            TryValidateModel(place);

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Organizer"))
                {
                    var currentUser = await _userManager.GetUserAsync(User);
                    @event.Organizers.Add(currentUser);
                }

                _context.Add(@event);

                foreach (var tagId in tags)
                {
                    var eventTag = _context.Tags.Find(tagId);
                    if (eventTag != null)
                        @event.Tags.Add(eventTag);
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
            return View(@event);
        }



        // GET: Events/Edit/5
        [Authorize(Roles = "Organizer, Moderator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
                    .ThenInclude(e => e.City)
                        .ThenInclude(e => e.Country)
                .Include(e => e.Place)
                    .ThenInclude(pt => pt.PlaceType)
                .Include(e => e.Organizers)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }


            var allTags = _context.Tags.ToList();

            ViewBag.SelectedTags = @event.Tags.Select(t => t.Id).ToList();
            //ViewBag.TagId = new SelectList(allTags, "Id", "Title");


            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name", @event.PlaceId);
            ViewData["TagId"] = new MultiSelectList(allTags, "Id", "Title");

            var organizers = await _userManager.GetUsersInRoleAsync("Organizer");
            var organizerList = organizers.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.OrganizationOrFullName
            }).ToList();

            ViewBag.SelectedOrganizers = @event.Organizers.Select(o => o.Id).ToList();

            ViewData["OrganizerId"] = organizerList;

            var countries = await _context.Countries.ToListAsync();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");

            var cities = await _context.Cities.ToListAsync();
            ViewBag.Cities = new SelectList(cities, "Id", "Name");

            var placesInCities = await _context.Places.Include(c => c.City).ThenInclude(c => c.Country).Include(pt => pt.PlaceType).ToListAsync();
            ViewBag.PlacesInCities = new SelectList(placesInCities, "Id", "Name");

            return View(@event);
        }



        // POST: Events/Edit/5
        [Authorize(Roles = "Organizer, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlaceId,Title,Description,Speaker,DateTimeStart,DateTimeEnd,Price,Capacity,PictureUrl")] Event @event, int[] tags, int[] organizers)
        {
            Place place = _context.Places.Include(pt => pt.PlaceType).Include(c => c.City).ThenInclude(c => c.Country).FirstOrDefault(p => p.Id == @event.PlaceId);
            @event.Place = place;
            ModelState.Clear();
            TryValidateModel(@event);

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
            //ViewData["OrganizerId"] = new SelectList(_context.Users.Where(u => u.Role.Name.ToLower() == "organizer"), "Id", "OrganizationOrFullName", @event.Organizers);
            var organizerss = await _userManager.GetUsersInRoleAsync("Organizer");
            // Формируем список организаторов для передачи в представление
            var organizerList = organizerss.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = o.OrganizationOrFullName // Или любое другое поле, которое вы хотите отобразить
            }).ToList();

            // Передаем список организаторов в представление
            ViewData["OrganizerId"] = organizerList;
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Organizer, Moderator")]
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
        [Authorize(Roles = "Organizer, Moderator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Organizers)
                .Include(e => e.Tags)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            var organizersToRemove = @event.Organizers.ToList();
            foreach (var organizer in organizersToRemove)
            {
                @event.Organizers.Remove(organizer);
            }

            var tagsToRemove = @event.Tags.ToList();
            foreach (var tag in tagsToRemove)
            {
                @event.Tags.Remove(tag);
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
