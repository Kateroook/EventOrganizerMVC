using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using EventOrganizerInfrastructure.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;


namespace EventOrganizerInfrastructure.Controllers
{
    //public class TicketsController
    //{
    //    private readonly DbeventOrganizerContext _context;

    //    public TicketsController(DbeventOrganizerContext context)
    //    {
    //        _context = context;
    //    }
    //    public IActionResult DownloadTicket(int registrationId)
    //    {
    //        var registration = _context.Registrations
    //            .Include(r => r.User)
    //            .Include(r => r.Event)
    //            .FirstOrDefault(r => r.Id == registrationId);

    //        if (registration == null)
    //        {
    //            return View(registration);
    //        }

    //        var user = registration.User;
    //        var @event = registration.Event;

    //        var pdfPath = "ticket.pdf";
    //        var writer = new PdfWriter(pdfPath);
    //        var pdf = new PdfDocument(writer);
    //        var document = new Document(pdf);

    //        document.Add(new Paragraph($"Имя: {user.FullName}"));
    //        document.Add(new Paragraph($"Email: {user.Email}"));
    //        document.Add(new Paragraph($"Подія: {@event.Title}"));
    //        //document.Add(new Paragraph($"Дата події: {@event.EventDate}"));
    //        // Добавьте любую другую информацию о событии и участнике по вашему усмотрению

    //        document.Close();

    //        byte[] fileBytes = System.IO.File.ReadAllBytes(pdfPath);
    //        System.IO.File.Delete(pdfPath);

    //        return new FileContentResult(fileBytes, "application/pdf")
    //        {
    //            FileDownloadName = "ticket.pdf"
    //        };

    //    }
    //}
}
