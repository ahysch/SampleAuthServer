using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleAuthServer.API.Core.Models;

namespace SampleAuthServer.API.Data.Configurations
{
	public class UserAppConfiguration : IEntityTypeConfiguration<UserApp>
	{
		public void Configure(EntityTypeBuilder<UserApp> builder)
		{
			builder.Property(x => x.City).HasMaxLength(50);
		}
	}
}