using Adressboken.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net.Mail;

namespace Adressboken.Controllers
{
   

    public class KundController : Controller
    {
        private readonly IMongoCollection<Kund> _addressCollection;
        private readonly IEmailSender emailSender;
        
        public KundController(IMongoDatabase database, IEmailSender emailSender)
        {
            _addressCollection = database.GetCollection<Kund>("addresses");
            this.emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var addresses = await _addressCollection.Find(_ => true).ToListAsync();
            return View(addresses);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Kund address)
        {
            await _addressCollection.InsertOneAsync(address);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var address = await _addressCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, Kund updatedAddress)
        {
            await _addressCollection.ReplaceOneAsync(a => a.Id == id, updatedAddress);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var address = await _addressCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (address == null)
            {
                return NotFound();
            }
            return View(address);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _addressCollection.DeleteOneAsync(a => a.Id == id);
            return RedirectToAction("Index");
        }

        // ... Övrig kod ...

        public async Task<IActionResult> ReparationKlar(string id)
        {
            var address = await _addressCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (address == null)
            {
                return NotFound();
            }

            if (!address.ReparationKlar)
            {
                address.ReparationKlar = true;

                // Uppdatera dokumentet med den nya statusen
                await _addressCollection.ReplaceOneAsync(a => a.Id == id, address);

                // Skicka e-post till kunden
                if (!string.IsNullOrEmpty(address.Email) && address.ReparationKlar)
                {
                    string subject = "Reparationen är klar";
                    string message = "Din reparation är klar. Vänligen kontakta oss för att hämta din bil.";

                    try
                    {
                        // Skicka e-post till kunden med din EmailSender
                        await emailSender.SendEmailAsync(address.Email, subject, message);
                    }
                    catch (Exception ex)
                    {
                        // Hantera eventuella fel vid sändning av e-post
                        // Logga felet eller visa felmeddelande på klienten
                    }
                }
            }

            return RedirectToAction("Index");
        }


    }
}
