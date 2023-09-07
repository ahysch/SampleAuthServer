using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Filters;
using System.Security.Claims;

namespace TestAppZ.API.Controllers
{
	
	[Route("api/[controller]")]
	[ApiController]
	public class SaleController : ControllerBase
	{
		// isAll kullanılmazsa default olarak false.Eğer tüm policyler uygulanmak istenirse isAll:true'ya set etmek gerek.
		[AuthorizeCustomPolicy(roles: "Admin", policies: "AnkaraPolicy,AgePolicy")]
		//[Authorize(Roles ="Admin",Policy ="AgePolicy")]
		[HttpGet]
		public IActionResult GetSale()
		{
			var userName = HttpContext.User.Identity.Name;

			var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

			return Ok($" Sale Process => UserName:{userName} - UserId :{userIdClaim.Value}");
		}
	}
}