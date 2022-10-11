using System;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
	public class LoginRequest
	{
		[Required]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }

		public string ReturnUrl { get; set; }
	}
}

