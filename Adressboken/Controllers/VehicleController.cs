using Adressboken.Models;
using Adressboken.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adressboken.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly RegCheckApiService _regCheckApiService;

        public VehicleController(RegCheckApiService regCheckApiService)
        {
            _regCheckApiService = regCheckApiService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchAsync(VehicleSearchViewModel viewModel)
        {
            string registrationNumber = viewModel.RegistrationNumber;

            // Anropa RegCheckApiService för att söka efter fordonets uppgifter
            var vehicleDetails = await _regCheckApiService.GetVehicleDetailsAsync(registrationNumber);

            if (vehicleDetails != null)
            {
                return View("SearchResult", vehicleDetails);
            }
            else
            {
                ViewBag.ErrorMessage = "No vehicle details found.";
                return View("Index");
            }
        }


    }
}
