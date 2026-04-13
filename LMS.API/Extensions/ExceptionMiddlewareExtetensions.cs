using Domain.Models.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace LMS.API.Extensions;

public static class ExceptionMiddlewareExtetensions
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async context =>
            {
                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = contextFeature?.Error;

                var problemDetailsFactory = app.Services.GetRequiredService<ProblemDetailsFactory>();

                var (statusCode, title, detail) = exception switch
                {
                    BadRequestException badRequest => (StatusCodes.Status400BadRequest, badRequest.Title, badRequest.Message),
                    NotFoundException notFound => (StatusCodes.Status404NotFound, notFound.Title, notFound.Message),
                    TokenValidationException tokenEx => (tokenEx.StatusCode, "Unauthorized", tokenEx.Message),
                    DomainException domainEx => (StatusCodes.Status400BadRequest, domainEx.Title, domainEx.Message),
                    null => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred."),
                    _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", exception.Message)
                };

                var problemDetails = problemDetailsFactory.CreateProblemDetails(
                    context,
                    statusCode: statusCode,
                    title: title,
                    detail: detail,
                    instance: context.Request.Path);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problemDetails);

            });
        });
    }
}
