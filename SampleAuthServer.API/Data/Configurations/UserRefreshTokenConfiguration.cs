using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SampleAuthServer.API.Core.Models;

namespace SampleAuthServer.API.Data.Configurations
{
	public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
	{
		public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
		{
			builder.HasKey(x => x.UserId);
			builder.Property(x => x.RefreshToken).IsRequired().HasMaxLength(200);
		}
	}
}