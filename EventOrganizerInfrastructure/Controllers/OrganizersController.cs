using ClosedXML.Excel;
using EventOrganizerDomain.Model;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
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
            var organizer = _userManager.GetUserAsync(User).Result;
            if (organizer == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
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

            ViewBag.EventId = eventId;

            return View(participants);
        }

        [Authorize(Roles = "Organizer")]
        public IActionResult DownloadParticipants(int eventId, string format)
        {
            
            var participants = _context.Registrations
                .Include(r => r.User)
                .Where(r => r.EventId == eventId)
                .Select(r => r.User)
                .ToList();

            var eventName = _context.Events.FirstOrDefault(e => e.Id == eventId)?.Title;
           
            if (format == "pdf")
            {
                // Создание PDF
                var pdfPath = $"participants_{eventName}.pdf";
                using (var stream = new MemoryStream())
                {
                    var writer = new PdfWriter(stream);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);
                    string fontPath = "P:\\Inter\\Inter-VariableFont_slnt,wght.ttf";

                    PdfFont font = PdfFontFactory.CreateFont(fontPath);

                    document.SetFont(font);
                    document.Add(new Paragraph($"Список учасників '{eventName}'")                      
                        .SetFontSize(16)
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetMarginBottom(10));

                    List list = new List()
                        .SetSymbolIndent(12)
                        .SetListSymbol("\u2022")                         
                        .SetFontSize(12)
                        .SetTextAlignment(TextAlignment.LEFT);

                    foreach (var participant in participants)
                    {
                        list.Add(new ListItem($"{participant.FullName} - {participant.Email}"));
                    }
                    document.Add(list);
                    document.Close();
                    return File(stream.ToArray(), "application/pdf", pdfPath);
                }
            }
            else if (format == "excel")
            {
                var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Participants");

                worksheet.Cell(1, 1).Value = "Ім'я учасника";
                worksheet.Cell(1, 2).Value = "Email";

                for (int i = 0; i < participants.Count; i++)
                {
                    worksheet.Cell(i + 2, 1).Value = participants[i].FullName;
                    worksheet.Cell(i + 2, 2).Value = participants[i].Email;
                }

                var excelPath = $"participants_event_{eventId}.xlsx";
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelPath);
                }
            }
            else
            {
                return BadRequest("Непідтримуємий формат");
            }
        }
    }
}
