using SampleAuthServer.API.Core.Configuration;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Models;

namespace SampleAuthServer.API.Core.Services
{
	public interface ITokenService
	{
		Task<TokenDto> CreateTokenAsync(UserApp userApp);

		ClientTokenDto CreateTokenByClient(Client client);
	}
}