using EventOrganizerDomain.Model;
using EventOrganizerInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventOrganizerInfrastructure.Controllers
{
    [Authorize(Roles = "Participant")]
    public class RegistrationController : Controller
    {
        private readonly DbeventOrganizerContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RegistrationController(DbeventOrganizerContext context,  UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;          
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var userId = int.Parse(_userManager.GetUserId(User));

            
            var registrations = _context.Registrations
                .Include(r => r.Event)
                .Where(r => r.UserId == userId)
                .ToList();

            return View(registrations);
        }


        [HttpGet]
        public IActionResult RegisterForEvent(int id)
        {
            var model = new RegisterForEventViewModel { EventId = id };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterForEvent(RegisterForEventViewModel model)
        {
            var eventName = _context.Events.FirstOrDefault(e => e.Id == model.EventId)?.Title;
            var user = _userManager.GetUserId(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingRegistration = _context.Registrations.FirstOrDefault(r => r.UserId == int.Parse(user) && r.EventId == model.EventId);
            if (existingRegistration != null)
            {

                ModelState.AddModelError(string.Empty, "Ви вже зареєстровані на цю подію!");
                
                return View(model);
            }

            var registration = new Registration
            {
                UserId = int.Parse(user),
                EventId = model.EventId,
                CreatedAt = DateTime.Now,
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] = $"Ви успішно зареєструвались на подію: \"{eventName}\"!";

            //return RedirectToAction("Index", "Events");
            var eventId = model.EventId;

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRegistration(int registrationId)
        {
            var registration = await _context.Registrations.FindAsync(registrationId);

            if (registration == null)
            {
                return NotFound();
            }

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
