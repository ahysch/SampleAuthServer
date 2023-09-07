using Microsoft.AspNetCore.Identity;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Models;
using SampleAuthServer.API.Core.Services;
using SampleAuthServer.API.Service.Mapping;
using SharedLibrary.Dtos;
using System.Net;

namespace SampleAuthServer.API.Service.Services
{
	public class UserService : IUserService
	{
		private readonly UserManager<UserApp> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
		{
			var user = new UserApp
			{
				Email = createUserDto.Email,
				UserName = createUserDto.UserName,
			};

			var result = await _userManager.CreateAsync(user, createUserDto.Password);

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(x => x.Description).ToList();

				return Response<UserAppDto>.Fail(new ErrorDto(errors, true), StatusCodes.Status400BadRequest);
			}

			return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
		}

		public async Task<Response<NoContentDto>> CreateUserRoles(string userName)
		{
			if (!await _roleManager.RoleExistsAsync("Admin"))
			{
				await _roleManager.CreateAsync(new() { Name = "Admin" });
				await _roleManager.CreateAsync(new() { Name = "Manager" });
			}


			var user = await _userManager.FindByNameAsync(userName);

			await _userManager.AddToRoleAsync(user, "Manager");

			return Response<NoContentDto>.Success(StatusCodes.Status201Created);
		}

		public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
		{
			var user = await _userManager.FindByNameAsync(userName);
			if (user == null)
				return Response<UserAppDto>.Fail("UserName not found", StatusCodes.Status404NotFound, true);

			return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), StatusCodes.Status200OK);
		}
	}
}