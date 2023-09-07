using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SampleAuthServer.API.Core.Configuration;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Models;
using SampleAuthServer.API.Core.Repositories;
using SampleAuthServer.API.Core.Services;
using SampleAuthServer.API.Core.UnitOfWork;
using SharedLibrary.Dtos;
using System.Net;

namespace SampleAuthServer.API.Service.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly List<Client> _clients;
		private readonly ITokenService _tokenService;
		private readonly UserManager<UserApp> _userManager;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

		public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
		{
			_clients = optionsClient.Value;
			_tokenService = tokenService;
			_userManager = userManager;
			_unitOfWork = unitOfWork;
			_userRefreshTokenService = userRefreshTokenService;
		}

		public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
		{
			if (loginDto == null)
				throw new ArgumentNullException(nameof(loginDto));

			var user = await _userManager.FindByEmailAsync(loginDto.Email);

			if (user == null)
				return Response<TokenDto>.Fail("Email or Password is wrong", StatusCodes.Status400BadRequest, true);

			if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
				return Response<TokenDto>.Fail("Email or Password is wrong", StatusCodes.Status400BadRequest, true);

			var tokenDto = await _tokenService.CreateTokenAsync(user);

			var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

			if (userRefreshToken == null)
			{
				await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, RefreshToken = tokenDto.RefreshToken, Expiration = tokenDto.RefreshTokenExpiration });
			}
			else
			{
				userRefreshToken.RefreshToken = tokenDto.RefreshToken;
				userRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;
			}

			await _unitOfWork.CommitAsync();

			return Response<TokenDto>.Success(tokenDto, StatusCodes.Status200OK);
		}

		public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
		{
			var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);

			if (client == null) return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", StatusCodes.Status404NotFound, true);

			var clientTokenDto = _tokenService.CreateTokenByClient(client);

			return Response<ClientTokenDto>.Success(clientTokenDto, StatusCodes.Status200OK);
		}

		public async Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken)
		{
			var existRefreshToken = await _userRefreshTokenService.Where(x => x.RefreshToken == refreshToken).SingleOrDefaultAsync();

			if (existRefreshToken == null)
				return Response<TokenDto>.Fail("Refresh token not found", StatusCodes.Status404NotFound, true);

			var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

			if (user == null)
				return Response<TokenDto>.Fail("UserId not found", StatusCodes.Status404NotFound, true);

			var tokenDto = await _tokenService.CreateTokenAsync(user);

			existRefreshToken.RefreshToken = tokenDto.RefreshToken;
			existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

			await _unitOfWork.CommitAsync();

			return Response<TokenDto>.Success(tokenDto, StatusCodes.Status200OK);
		}

		public async Task<Response<NoContentDto>> RevokeRefreshTokenAsync(string refreshToken)
		{
			var existRefreshToken = await _userRefreshTokenService.Where(x => x.RefreshToken == refreshToken).SingleOrDefaultAsync();

			if (existRefreshToken == null)
				return Response<NoContentDto>.Fail("Refresh token not found", StatusCodes.Status404NotFound, true);

			_userRefreshTokenService.Remove(existRefreshToken);

			await _unitOfWork.CommitAsync();

			return Response<NoContentDto>.Success(StatusCodes.Status200OK);
		}
	}
}