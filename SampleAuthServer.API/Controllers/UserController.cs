using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Services;
using SharedLibrary.Exceptions;

namespace SampleAuthServer.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : CustomBaseController
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
		{
			var result = await _userService.CreateUserAsync(createUserDto);

			return ActionResultInstance(result);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetUser()
		{
			var user = await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name);

			return ActionResultInstance(user);
		}

		[HttpPost("CreateUserRoles/{userName}")]
		public async Task<IActionResult> CreateUserRoles(string userName)
		{
			return ActionResultInstance(await _userService.CreateUserRoles(userName));
		}
	}
}