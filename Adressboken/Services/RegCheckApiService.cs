using Adressboken.Models;
using System.Text.Json;
using System.Xml.Linq;

namespace Adressboken.Services
{
    public class RegCheckApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _username = "Sandung"; // Lägg till ditt användarnamn här

        public RegCheckApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("RegCheckApi");
            _httpClient.BaseAddress = new Uri("https://www.registreringsnummerapi.se/api/reg.asmx/");
        }

        public async Task<RegCheckApiResponse> GetVehicleDetailsAsync(string registrationNumber)
        {
            string apiUrl = $"CheckSweden?RegistrationNumber={registrationNumber}&username={_username}";

            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var contentStream = await response.Content.ReadAsStreamAsync();
                XDocument xdoc = XDocument.Load(contentStream);

                XNamespace ns = "http://regcheck.org.uk";
                XElement vehicleJsonElement = xdoc.Root.Element(ns + "vehicleJson");
                
                if (vehicleJsonElement != null)
                {
                    string vehicleJsonString = vehicleJsonElement.Value;
                    RegCheckApiResponse vehicleDetails = JsonSerializer.Deserialize<RegCheckApiResponse>(vehicleJsonString);
                    return vehicleDetails;
                }
            }

            return null;
        }


    }
}
