using System;
using Microsoft.AspNetCore.Authorization;

namespace Identity.CustomPolicy
{
	public class AllowUserPolicy : IAuthorizationRequirement
	{
		public string[] AllowUser { get; set; }

		public AllowUserPolicy(params string[] users)
		{
			AllowUser = users;
		}
	}
}

