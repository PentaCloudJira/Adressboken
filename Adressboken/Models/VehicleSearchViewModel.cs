namespace Adressboken.Models
{
    public class VehicleSearchViewModel
    {
        public string? Regnummer { get; set; }
        public RegCheckApiResponse VehicleDetails { get; set; }
    }
}
