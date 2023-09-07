using Microsoft.AspNetCore.Mvc;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Services;

namespace SampleAuthServer.API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthController : CustomBaseController
	{
		private readonly IAuthenticationService _authenticationService;

		public AuthController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		[HttpPost]
		public async Task<IActionResult> CreateToken(LoginDto loginDto)
		{
			var result = await _authenticationService.CreateTokenAsync(loginDto);

			return ActionResultInstance(result);
		}

		[HttpPost]
		public IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
		{
			var result = _authenticationService.CreateTokenByClient(clientLoginDto);

			return ActionResultInstance(result);
		}

		[HttpPost]
		public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
		{
			var result = await _authenticationService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken);

			return ActionResultInstance(result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
		{
			var result = await _authenticationService.CreateTokenByRefreshTokenAsync(refreshTokenDto.RefreshToken);

			return ActionResultInstance(result);
		}
	}
}