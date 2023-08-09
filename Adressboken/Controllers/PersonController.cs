using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Adressboken.Controllers
{
    public class PersonController : Controller
    {
        private readonly IMongoCollection<Person> _addressCollection;

        public PersonController(IMongoDatabase database)
        {
            _addressCollection = database.GetCollection<Person>("addresses");
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
        public async Task<IActionResult> Create(Person address)
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
        public async Task<IActionResult> Edit(string id, Person updatedAddress)
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

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _addressCollection.DeleteOneAsync(a => a.Id == id);
            return RedirectToAction("Index");
        }
    }
}
