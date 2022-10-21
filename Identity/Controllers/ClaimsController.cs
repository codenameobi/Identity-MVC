using System;
using System.Security.Claims;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
	[Authorize]
	public class ClaimsController : Controller
	{
		private UserManager<AppUser> _userManager;
		private IAuthorizationService _authService;

		public ClaimsController(UserManager<AppUser> userManager, IAuthorizationService authService)
		{
			_userManager = userManager;
			_authService = authService;
		}

		public ViewResult Index() => View(User?.Claims);

		public ViewResult Create() => View();

		[HttpPost]
		[ActionName("Create")]
		public async Task<IActionResult> Create_Post(string claimType, string claimValue)
		{
			AppUser user = await _userManager.GetUserAsync(HttpContext.User);
			Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);
			IdentityResult result = await _userManager.AddClaimAsync(user, claim);

			if (result.Succeeded)
				return RedirectToAction("Index");
			else
				Errors(result);

			return View();
		}

		public async Task<IActionResult> Delete(string claimValues)
		{
			AppUser user = await _userManager.GetUserAsync(HttpContext.User);

			string[] claimValuesArray = claimValues.Split(";");
			string claimType = claimValuesArray[0], claimValue = claimValuesArray[1], claimIssuer = claimValuesArray[2];

			Claim claim = User.Claims.Where(x => x.Type == claimType && x.Value == claimValue && x.Issuer == claimIssuer).FirstOrDefault();

			IdentityResult result = await _userManager.RemoveClaimAsync(user, claim);

			if (result.Succeeded)
				return RedirectToAction("Index");
			else
				Errors(result);

			return View("Index");
		}

        void Errors(IdentityResult result)
        {
			foreach (IdentityError error in result.Errors)
				ModelState.AddModelError("", error.Description);
        }

		//only authorize users that match the AspManager policy
		[Authorize(Policy = "AspManager")]
		public IActionResult Project() => View("Index", User?.Claims);

		//only authorize user with name Tom
		[Authorize(Policy = "AllowTom")]
		public ViewResult TomFiles() => View("Index", User?.Claims);

		public async Task<IActionResult> PrivateAccess(string title)
		{
			string[] allowedUsers = { "admin", "user" };
			AuthorizationResult authorized = await _authService.AuthorizeAsync(User, allowedUsers, "PrivateAccess");

			if (authorized.Succeeded)
				return View("Index", User?.Claims);
			else
				return new ChallengeResult();
		}
    }
}

