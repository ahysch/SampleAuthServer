using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
	public static class CustomExceptionHandler
	{
		public static void UseCustomExceptionHandler(this IApplicationBuilder app)
		{
			app.UseExceptionHandler(config =>
			{
				config.Run(async context =>
				{
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					context.Response.ContentType = "application/json";

					var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
					if (errorFeature != null)
					{
						var ex = errorFeature.Error;

						ErrorDto errorDto = null;

						if (ex is CustomException)
						{
							errorDto = new(ex.Message, true);
						}
						else
						{
							errorDto = new(ex.Message, false);
						}

						var response = Response<NoContentDto>.Fail(errorDto, StatusCodes.Status500InternalServerError);

						await context.Response.WriteAsJsonAsync(response);
					}
				});
			});
		}
	}
}
