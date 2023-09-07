using AutoMapper;
using SampleAuthServer.API.Core.Dtos;
using SampleAuthServer.API.Core.Models;

namespace SampleAuthServer.API.Service.Mapping
{
	public class AutoMapping : Profile
	{
		public AutoMapping()
		{
			CreateMap<ProductDto, Product>().ReverseMap();
			CreateMap<UserAppDto, UserApp>().ReverseMap();
		}
	}
}