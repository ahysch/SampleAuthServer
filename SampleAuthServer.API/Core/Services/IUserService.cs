using SampleAuthServer.API.Core.Dtos;
using SharedLibrary.Dtos;

namespace SampleAuthServer.API.Core.Services
{
	public interface IUserService
	{
		Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);

		Task<Response<UserAppDto>> GetUserByNameAsync(string userName);

		Task<Response<NoContentDto>> CreateUserRoles(string userName);
	}
}