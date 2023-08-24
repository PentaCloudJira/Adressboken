using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Adressboken.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Adressboken.Controllers;
public class AccountController : Controller
{
    // Mocked user data
    private const string MockedUsername = "bengt";
    private const string MockedPassword = "trabant"; // Ev byt ut senare mot en hashad variant?
    public IActionResult Login()
    {
    return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken] 
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
        // Check model validators
        if (!ModelState.IsValid)
        {
            return View(model);
        }
            // Mocked user verification - replace with real authentication
            if (model.Username == MockedUsername && model.Password == MockedPassword)
        {
            // Set up the session/cookie for the authenticated user.
            var claims = new[] { new Claim(ClaimTypes.Name, model.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Kund"); // Bytte ut "Index", "Home" mot "Index", "Kund" för att komma till Kundregistret
        }
            ModelState.AddModelError(string.Empty, "Nu blev det knas. Försök igen"); // Generellt felmeddelande
            return View(model);
    }
    public IActionResult CognitoLogin()
    {
        return Challenge(
        new AuthenticationProperties
        {
        RedirectUri = Url.Action("Index", "Kund")
        },
        OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Authorize]
    public IActionResult Logout()
    {
        return SignOut(
        new AuthenticationProperties
        {
            RedirectUri = Url.Action("Index", "Home")
        },
        CookieAuthenticationDefaults.AuthenticationScheme,
        OpenIdConnectDefaults.AuthenticationScheme);
    }

}