using System;
using System.ComponentModel.DataAnnotations;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
	public class RoleController : Controller
	{
		private RoleManager<IdentityRole> _roleManager;
		private UserManager<AppUser> _userManager;

		public RoleController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public ViewResult Index() => View(_roleManager.Roles);

		private void Errors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
				ModelState.AddModelError("", error.Description);
		}

		public IActionResult Create() => View();

		[HttpPost]
		public async Task<IActionResult> Create([Required] string name)
		{
			if (ModelState.IsValid)
			{
				IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
				if (result.Succeeded)
					return RedirectToAction("Index");
				else
					Errors(result);
			}
			return View(name);
		}

		public async Task<IActionResult> Delete(string id)
		{
			IdentityRole role = await _roleManager.FindByIdAsync(id);
			if (role != null)
			{
				IdentityResult result = await _roleManager.DeleteAsync(role);
				if (result.Succeeded)
					return RedirectToAction("Index");
				else
					Errors(result);
			}
			else
				ModelState.AddModelError("", "No role found");
			return View("Index", _roleManager.Roles);
		}

		public async Task<IActionResult> Update(string id)
		{
			IdentityRole role = await _roleManager.FindByIdAsync(id);
			List<AppUser> members = new List<AppUser>();
			List<AppUser> nonMembers = new List<AppUser>();
			foreach (AppUser user in _userManager.Users)
			{
				var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
				list.Add(user);
			}
			return View(new RoleEdit
			{
				Role = role,
				Members = members,
				NonMembers = nonMembers
			});
		}

		[HttpPost]
		public async Task<IActionResult> Update(RoleModification model)
		{
			IdentityResult result;
			if (ModelState.IsValid)
			{
				foreach (string userId in model.AddIds ?? new string[] {})
				{
					AppUser user = await _userManager.FindByIdAsync(userId);
					if (user != null)
					{
						result = await _userManager.AddToRoleAsync(user, model.RoleName);
						if (!result.Succeeded)
							Errors(result);
					}
				}
				foreach (string userId in model.DeleteIds ?? new string[] { })
				{
					AppUser user = await _userManager.FindByIdAsync(userId);
					if (user != null)
					{
						result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
						if (!result.Succeeded)
							Errors(result);
					}
				}
			}

			if (ModelState.IsValid)
				return RedirectToAction(nameof(Index));
			else
				return await Update(model.RoleId);
		}
	}
}

