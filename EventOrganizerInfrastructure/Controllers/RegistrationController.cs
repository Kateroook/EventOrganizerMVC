using EventOrganizerDomain.Model;
using EventOrganizerInfrastructure.ViewModel;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iText.Layout;
using System.IO;
using EventOrganizerInfrastructure.Models;
using System.Diagnostics;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Font;
using iText.IO.Font.Constants;




namespace EventOrganizerInfrastructure.Controllers
{
    [Authorize(Roles = "Participant")]
    public class RegistrationController : Controller
    {
        private readonly DbeventOrganizerContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public RegistrationController(DbeventOrganizerContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
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

            TempData["LastName"] = model.LastName;
            TempData["FirstName"] = model.FirstName;
            TempData["Email"] = model.Email;

            TempData["SuccessMessage"] = $"Ви успішно зареєструвались на подію: \"{eventName}\"!";
            TempData["RegistrationId"] = registration.Id;

            var eventId = model.EventId;

            return RedirectToAction("Success");
        }
        public IActionResult Success()
        {
            if (!TempData.ContainsKey("SuccessMessage") || !TempData.ContainsKey("RegistrationId"))
            {
                return RedirectToAction("Index", "Events"); // Редирект на главную страницу, если отсутствует информация о регистрации
            }

            var successMessage = TempData["SuccessMessage"].ToString();
            var registrationId = Convert.ToInt32(TempData["RegistrationId"]);

            TempData.Remove("SuccessMessage");
            TempData.Remove("RegistrationId");

            return View(new { SuccessMessage = successMessage, RegistrationId = registrationId });
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


        public IActionResult DownloadTicket(int registrationId)
        {
            var registration = _context.Registrations
                .Include(r => r.User)
                .Include(r => r.Event)
                    .ThenInclude(p => p.Place)
                .FirstOrDefault(r => r.Id == registrationId);

            if (registration == null)
            {
                return View(registration);
            }

            string lastName = TempData["LastName"] as string;
            string firstName = TempData["FirstName"] as string;
            string email = TempData["Email"] as string;

            TempData.Remove("LastName");
            TempData.Remove("FirstName");

            var @event = registration.Event;

            var pdfPath = "ticket_{@event.Title}.pdf";
            var writer = new PdfWriter(pdfPath);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            string fontPath = "P:\\Inter\\Inter-VariableFont_slnt,wght.ttf";
            string fontAwesomePath = "P:\\fontawesome-free-6.5.2-desktop\\fontawesome-free-6.5.2-desktop\\otfs\\Font Awesome 6 Free-Solid-900.otf";
            
            PdfFont font = PdfFontFactory.CreateFont(fontPath);
            PdfFont fontAwesomeFont = PdfFontFactory.CreateFont(fontAwesomePath, PdfEncodings.IDENTITY_H);

            FontProgramFactory.RegisterFont(fontPath);

           // document.SetFont(font);
            document.Add(new Paragraph(@event.Title)
                .SetFont(font)
                .SetFontSize(16)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10));

            Paragraph dateTimeParagraph = new Paragraph()
                .Add(new Text("\uf073 ").SetFont(fontAwesomeFont))
                .Add(new Text(@event.EventDate).SetFont(font))               
                .Add(new Text("   \uf017 ").SetFont(fontAwesomeFont))
                .Add(new Text(@event.EventTime).SetFont(font)); 

         
            document.Add(dateTimeParagraph);

            
            Paragraph placeNameParagraph = new Paragraph()
                .Add(new Text("\uf3c5 ").SetFont(fontAwesomeFont))
                .Add(new Text(@event.Place.Name).SetFont(font)); 

            // Добавляем параграф названия места проведения в документ
            document.Add(placeNameParagraph);

            document.Add(new LineSeparator(new SolidLine())
                .SetMarginTop(10)
                .SetMarginBottom(10));

            document.Add(new Paragraph($"Прізвище: {lastName}").SetFont(font));
            document.Add(new Paragraph($"Ім'я: {firstName}").SetFont(font));
            document.Add(new Paragraph($"Email: {email}"));

            document.Add(new LineSeparator(new SolidLine())
                .SetMarginTop(10)
                .SetMarginBottom(10));

            document.Add(new Paragraph($"{@event.Place.Name}").SetFont(font));
            document.Add(new Paragraph($"Адреса: {@event.Place.Address}").SetFont(font));

            document.Add(new LineSeparator(new SolidLine())
                .SetMarginTop(10)
                .SetMarginBottom(10));

            document.Add(new Paragraph($"Ціна: {@event.Price}").SetFont(font));

            document.Add(new LineSeparator(new SolidLine())
                .SetMarginTop(10)
                .SetMarginBottom(10));

            document.Add(new Paragraph($"RYTMY.CO")
                .SetMarginTop(20));

            document.Close();

            byte[] fileBytes = System.IO.File.ReadAllBytes(pdfPath);
            System.IO.File.Delete(pdfPath);

            return new FileContentResult(fileBytes, "application/pdf")
            {
                FileDownloadName = "ticket_{@event.Title}.pdf"
            };

        }

    }
}
