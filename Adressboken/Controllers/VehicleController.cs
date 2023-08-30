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
        public async Task<IActionResult> Search(VehicleSearchViewModel viewModel)
        {
            if (!string.IsNullOrEmpty(viewModel.Regnummer))
            {
                string registrationNumber = viewModel.Regnummer;

                // Anropa RegCheckApiService för att söka efter fordonets uppgifter
                var vehicleDetails = await _regCheckApiService.GetVehicleDetailsAsync(registrationNumber);

                viewModel.VehicleDetails = vehicleDetails;

                if (vehicleDetails == null)
                {
                    ViewBag.ErrorMessage = "No vehicle details found.";
                }
            }
            else
            {
                viewModel.VehicleDetails = null;
            }

            return View("Index", viewModel);
        }






    }
}
