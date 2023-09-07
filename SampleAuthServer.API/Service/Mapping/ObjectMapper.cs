using AutoMapper;

namespace SampleAuthServer.API.Service.Mapping
{
	public class ObjectMapper
	{
		private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
		{
			var config = new MapperConfiguration(config =>
			{
				config.AddProfile<AutoMapping>();
			});

			return config.CreateMapper();
		});

		public static IMapper Mapper => lazy.Value;
	}
}