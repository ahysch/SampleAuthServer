using FluentValidation;
using SampleAuthServer.API.Core.Dtos;

namespace SampleAuthServer.API.Validations
{
	public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
	{
		public CreateUserDtoValidator()
		{
			RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Invalid Email adress.");

			RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");

			RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required").MinimumLength(5).WithMessage("Username minimum length is must be 5 characters");
		}
	}
}