using Microsoft.AspNetCore.Authorization;
using SharedLibrary.Configuration;
using SharedLibrary.Extensions;
using SharedLibrary.Filters;
using TestAppZ.API.Requirements;
using static TestAppZ.API.Requirements.BirthDateRequirement;
using BirthDateRequirement = TestAppZ.API.Requirements.BirthDateRequirement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();
builder.Services.AddCustomTokenAuth(tokenOptions);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAuthorizationHandler, BirthDateRequirementHandler>();
builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AnkaraPolicy", policy =>
	{
		policy.RequireClaim("city", "ankara");
		
	});
	options.AddPolicy("AgePolicy", policy =>
	{
		policy.Requirements.Add(new BirthDateRequirement(10));
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();