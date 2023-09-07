using SampleAuthServer.API.Core.Dtos;
using SharedLibrary.Dtos;

namespace SampleAuthServer.API.Core.Services
{
	public interface IAuthenticationService
	{
		Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);

		Task<Response<TokenDto>> CreateTokenByRefreshTokenAsync(string refreshToken);

		Task<Response<NoContentDto>> RevokeRefreshTokenAsync(string refreshToken);

		Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
	}
}