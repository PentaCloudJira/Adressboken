using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Adressboken.Controllers
{
    public class KundController : Controller
    {
        private readonly IMongoCollection<Kund> _addressCollection;

        public KundController(IMongoDatabase database)
        {
            _addressCollection = database.GetCollection<Kund>("addresses");
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
        [HttpGet]
       public async Task<IActionResult> Search(string query)
{
    if (query == null)
    {
        // Handle the case where query is null, such as showing all results
        var allResults = await _addressCollection.Find(_ => true).ToListAsync();
        return PartialView("_SearchResults", allResults);
    }

    var searchResults = await _addressCollection.Find(a =>
        (a.Namn != null && a.Namn.Contains(query)) ||
        (a.Adress != null && a.Adress.Contains(query)) ||
        (a.Telefonnummer != null && a.Telefonnummer.Contains(query)) ||
        (a.Email != null && a.Email.Contains(query)) ||
        (a.Bilmodell != null && a.Bilmodell.Contains(query)) ||
        (a.Årsmodell != null && a.Årsmodell.Contains(query))).ToListAsync();

    return PartialView("_SearchResults", searchResults);
}
    }
}
