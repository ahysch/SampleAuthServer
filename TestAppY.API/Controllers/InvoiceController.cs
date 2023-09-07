using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TestAppY.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class InvoiceController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetInvoice()
		{
			var userName = HttpContext.User.Identity.Name;

			var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

			return Ok($" Invoice Process => UserName:{userName} - UserId :{userIdClaim.Value}");
		}
	}
}