using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Identity.Models
{
    public class AppUser : IdentityUser
	{
		public Country Country { get; set; }
		public int Age { get; set; }
		[Required]
		public string Salary { get; set; }
	}
}

