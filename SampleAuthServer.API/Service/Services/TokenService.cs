using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SampleAuthServer.API.Core.Configuration;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Models;
using SampleAuthServer.API.Core.Services;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SampleAuthServer.API.Service.Services
{
	public class TokenService : ITokenService
	{
		private readonly UserManager<UserApp> _userManager;
		private readonly CustomTokenOption _tokenOption;

		public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
		{
			_userManager = userManager;
			_tokenOption = options.Value;
		}

		private string CreateRefreshToken()
		{
			var numberByte = new byte[32];
			using var random = RandomNumberGenerator.Create();
			random.GetBytes(numberByte);

			return Convert.ToBase64String(numberByte);
		}

		private async Task<IEnumerable<Claim>> GetClaims(UserApp userApp, List<string> audiences)
		{
			var userRoles = await _userManager.GetRolesAsync(userApp);

			var userList = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier,userApp.Id),
				new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
				new Claim(ClaimTypes.Name,userApp.UserName),
				new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
				new Claim("city",userApp.City),
				new Claim("birthdate",userApp.BirthDate.Value.ToShortDateString())
			};

			userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
			userList.AddRange(userRoles.Select(x=>new Claim(ClaimTypes.Role,x)));

			return userList;
		}

		private IEnumerable<Claim> GetClaimsByClient(Client client)
		{
			var claims = new List<Claim>();

			claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
			claims.Add(new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString()));

			return claims;
		}

		public async Task<TokenDto> CreateTokenAsync(UserApp userApp)
		{
			var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
			var refreshTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);
			var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

			SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

			JwtSecurityToken jwtSecurityToken = new(issuer: _tokenOption.Issuer, expires: accessTokenExpiration, notBefore: DateTime.Now, claims: await GetClaims(userApp, _tokenOption.Audience), signingCredentials: signingCredentials);

			var handler = new JwtSecurityTokenHandler();

			var token = handler.WriteToken(jwtSecurityToken);

			var tokenDto = new TokenDto
			{
				AccessToken = token,
				RefreshToken = CreateRefreshToken(),
				AccessTokenExpiration = accessTokenExpiration,
				RefreshTokenExpiration = refreshTokenExpiration
			};

			return tokenDto;
		}

		public ClientTokenDto CreateTokenByClient(Client client)
		{
			var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
			var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

			SigningCredentials signingCredentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

			JwtSecurityToken jwtSecurityToken = new(issuer: _tokenOption.Issuer, expires: accessTokenExpiration, notBefore: DateTime.Now, claims: GetClaimsByClient(client), signingCredentials: signingCredentials);

			var handler = new JwtSecurityTokenHandler();

			var token = handler.WriteToken(jwtSecurityToken);

			var tokenDto = new ClientTokenDto
			{
				AccessToken = token,
				AccessTokenExpiration = accessTokenExpiration
			};

			return tokenDto;
		}
	}
}