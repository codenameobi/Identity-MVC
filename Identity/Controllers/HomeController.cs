using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Identity.Controllers;

public class HomeController : Controller
{
    private UserManager<AppUser> _userManager;

    public HomeController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Roles = "Users")]
    public async Task<IActionResult> Index()
    {
        AppUser user = await _userManager.GetUserAsync(HttpContext.User);
        string message = "Hello " + user.UserName;
        return View((object)message);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}

