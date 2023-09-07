using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TestAppZ.API.Requirements
{
	public class BirthDateRequirement : IAuthorizationRequirement
	{
		public int Age { get; set; }

		public BirthDateRequirement(int age)
		{
			Age = age;
		}

		public class BirthDateRequirementHandler : AuthorizationHandler<BirthDateRequirement>
		{
			protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BirthDateRequirement requirement)
			{
				var birthdate = context.User.FindFirstValue(ClaimTypes.DateOfBirth);

				if (birthdate == null)
				{
					context.Fail();
					return Task.CompletedTask;
				}

				var today = DateTime.Now;

				var age = today.Year - Convert.ToDateTime(birthdate).Year;

				if (age >= requirement.Age)
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
}
