using EventOrganizerDomain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventOrganizerInfrastructure.Controllers
{
    public class OrganizersController : Controller
    {
        private readonly DbeventOrganizerContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        
        public OrganizersController(DbeventOrganizerContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "Organizer")]
        public IActionResult MyEvents()
        {
            
            // Получаем текущего организатора
            var organizer = _userManager.GetUserAsync(User).Result;
            if (organizer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Получаем список всех событий, созданных текущим организатором
            
            var events = _context.Events
                .Include(e => e.Place).ThenInclude(p => p.City)
                .Where(e => e.Organizers.Any(o => o.Id == organizer.Id));

            return View(events);
        }

        [Authorize(Roles = "Organizer")]
        public IActionResult Participants(int eventId)
        {
            var participants = _context.Registrations
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .Select(r => r.User)
                .ToList();

            return View(participants);
        }
    }
}
