using System;
using Microsoft.AspNetCore.Authorization;

namespace Identity.CustomPolicy
{
	public class AllowPrivatePolicy : IAuthorizationRequirement
	{
	}

    public class AllowPrivateHandler : AuthorizationHandler<AllowPrivatePolicy>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllowPrivatePolicy requirement)
        {
            string[] allowedUsers = context.Resource as string[];

            if (allowedUsers.Any(user => user.Equals(context.User.Identity.Name, StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}

