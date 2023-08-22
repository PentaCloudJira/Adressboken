using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Adressboken.Models;
namespace Adressboken.Controllers;
public class AccountController : Controller
{
// Mocked user data
private const string MockedUsername = "bengt";
private const string MockedPassword = "trabant"; // OBS! Byt ut senare mot en hashad variant.
public IActionResult Login()
{
return View();
}
[HttpPost]
[ValidateAntiForgeryToken] 
public IActionResult Login(LoginViewModel model)
{
// Check model validators
if (!ModelState.IsValid)
{
return View(model);
}
// Mocked user verification - replace with real authentication
if (model.Username == MockedUsername && model.Password == MockedPassword)
{

return RedirectToAction("Index", "Kund"); // Bytte ut "Index", "Home" mot "Index", "Kund" för att komma till Kundregistret
}
ModelState.AddModelError(string.Empty, "Nu blev det knas. Försök igen"); // Generellt felmeddelande
return View(model);
}
public IActionResult SecretInfo()   //Ej skapat View/SecretInfo utan styr till Kundregister ovan, denna action behövs inte?
{
return View();
}
}