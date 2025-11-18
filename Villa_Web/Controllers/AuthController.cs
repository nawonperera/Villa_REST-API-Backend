using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Villa_Utility;
using Villa_Web.Model.Dto;
using Villa_Web.Models;
using Villa_Web.Service.IServices;

namespace Villa_Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        LoginRequestDto obj = new();
        return View(obj);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto obj)
    {
        APIResponse response = await _authService.LoginAsync<APIResponse>(obj);
        if (response != null && response.IsSuccess)
        {
            LoginResponseDto model = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.Name, model.User.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, model.User.Role));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Store the token in session.
            HttpContext.Session.SetString(SD.SessionToken, model.Token);
            return RedirectToAction("Index", "Home"); //Redirect to home page after successful login.
        }
        else
        {
            ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
            return View(obj);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrationRequestDto obj)
    {
        APIResponse response = await _authService.RegisterAsync<APIResponse>(obj);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction("Login");
        }
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        //HttpContext comes from ControllerBase class.It encapsulates all HTTP-specific information about an individual HTTP request.
        //Like headers, cookies, session, user info etc. And it also manaeges requests and responses.
        await HttpContext.SignOutAsync();
        // Clear the session token.
        HttpContext.Session.SetString(SD.SessionToken, "");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
