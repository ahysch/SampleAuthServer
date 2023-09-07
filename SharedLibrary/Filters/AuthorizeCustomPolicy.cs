using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SharedLibrary.Filters
{
	/// <summary>
	/// Specifies that the class or method that this attribute is applied to requires 
	/// authorization based on user passing any one policy in the provided list of policies.
	/// </summary>
	public class AuthorizeCustomPolicy : TypeFilterAttribute
	{
		/// <summary>
		/// Initializes a new instance of the AuthorizeOnAnyOnePolicyAttribute class.
		/// </summary>
		/// <param name="policies">A comma delimited list of policies that are allowed to access the resource.</param>
		public AuthorizeCustomPolicy(string roles, string policies, bool isAll = false) : base(typeof(AuthorizeCustomPolicyFilter))
		{
			Regex commaDelimitedWhitespaceCleanup = new Regex("\\s+,\\s+|\\s+,|,\\s+",
					RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

			Arguments = new object[] { roles, policies, isAll };
		}
	}
	public class AuthorizeCustomPolicyFilter : IAsyncAuthorizationFilter
	{
		private readonly IAuthorizationService _authorization;
		public string Policies { get; private set; }
		public string Roles { get; private set; }
		public bool IsAll { get; private set; }

		/// <summary>
		/// Initializes a new instance of the AuthorizeOnAnyOnePolicyFilter class.
		/// </summary>
		/// <param name="policies">A comma delimited list of policies that are allowed to access the resource.</param>
		/// <param name="authorization">The AuthorizationFilterContext.</param>
		public AuthorizeCustomPolicyFilter(string roles, string policies, bool isAll, IAuthorizationService authorization)
		{
			Roles = roles;
			Policies = policies;
			IsAll = isAll;
			_authorization = authorization;
		}

		/// <summary>
		/// Called early in the filter pipeline to confirm request is authorized.
		/// </summary>
		/// <param name="context">A context for authorization filters i.e. IAuthorizationFilter and IAsyncAuthorizationFilter implementations.</param>
		/// <returns>Sets the context.Result to ForbidResult() if the user fails all of the policies listed.</returns>
		public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var policies = Policies.Split(",").ToList();
			var isInRole = context.HttpContext.User.IsInRole(Roles);
			//Rol kontrolü yapılıyor.
			if (!isInRole)
			{
				context.Result = new ForbidResult();
				return;
			}

			//Tüm policyler uygulanmak istendiğinde.
			if (IsAll)
			{
				foreach (var policy in policies)
				{
					var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, policy);
					if (!authorized.Succeeded)
					{
						context.Result = new ForbidResult();
						return;
					}
				}
			}
			// Herhangi bir policy uygulanmak istendiğinde.
			else
			{
				foreach (var policy in policies)
				{
					var authorized = await _authorization.AuthorizeAsync(context.HttpContext.User, policy);
					if (authorized.Succeeded)
					{
						return;
					}
				}

				context.Result = new ForbidResult();
				return;
			}

		}
	}
}
