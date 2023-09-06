using System.Security.Cryptography;

namespace Adressboken.Models
{
    public class RegCheckApiResponse
    {
        public string? ABICode { get; set; }
        public string? Description { get; set; }
        public string? RegistrationYear { get; set; }
        public CarProperty? CarMake { get; set; }
        public CarProperty? CarModel { get; set; }
        public CarProperty? EngineSize { get; set; }
        public CarProperty? FuelType { get; set; }
        public CarProperty? BodyStyle { get; set; }
        public string? Colour { get; set; }
        public string? RegistrationDate { get; set; }
        public CarProperty? MakeDescription { get; set; }
        public CarProperty? ModelDescription { get; set; }
        public CarProperty? Immobiliser { get; set; }
        public CarProperty? NumberOfSeats { get; set; }
        public CarProperty? DriverSide { get; set; }
        public string? VechileIdentificationNumber { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CarProperty
    {
        public string? CurrentTextValue { get; set; }
    }

}
