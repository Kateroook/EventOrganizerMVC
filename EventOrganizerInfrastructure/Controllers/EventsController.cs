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
            ViewData["Tags"] = new SelectList(_context.Tags, "Id", "Name");
            ViewBag.Organizers = new SelectList(_context.Users.Where(u => u.Role.Name == "organizer"), "Id", "FullName" ,"OrganizationName");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaceId,Title,Description,Speaker,DateTimeStart,DateTimeEnd,Price,Capacity,CreatedAt,LastUpdatedAt,PictureUrl,Id")] Event @event, int[] tags, int[] organizers, int[] comments, int[] regs)
        {



            if (ModelState.IsValid)
            {
                _context.Add(@event);
                foreach(var tagId  in tags)
                {
                    var eventTag = new Tag { Id = tagId };
                    @event.Tags.Add(eventTag);            
                }
                    @event.CreatedAt = DateTime.Now;
                    @event.LastUpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name", @event.PlaceId);
            ViewData["TagId"] = new SelectList(_context.Tags, "Id", "Title");
            return View(@event);
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name", @event.PlaceId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlaceId,Title,Description,Speaker,DateTimeStart,DateTimeEnd,Price,Capacity,CreatedAt,LastUpdatedAt, PictureUrl, Id")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
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
            ViewData["PlaceId"] = new SelectList(_context.Places, "Id", "Name" + "CityId", @event.PlaceId);
            return View(@event);
        }

        //[HttpPost]
        //public async Task<IActionResult> UploadCover(int eventId, IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("Файл не выбран");

        //    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/events");
        //    var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
        //    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    var eventToUpdate = await _context.Events.FindAsync(eventId);
        //    if (eventToUpdate == null)
        //    {
        //        return NotFound();
        //    }

        //    eventToUpdate.ImageUrl = "/uploads/" + uniqueFileName;
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Index");
        //}
        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Place)
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
