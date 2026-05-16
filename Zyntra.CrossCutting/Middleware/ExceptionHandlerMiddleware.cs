using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Zyntra.Shared.Models;

namespace Zyntra.CrossCutting.Middleware;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteResponse(context, 400, new ErrorResponse
            {
                Type = "ValidationError",
                Message = "Dados inválidos.",
                Errors = ex.Errors.Select(e => e.ErrorMessage).ToList(),
                StatusCode = 400
            });
        }
        catch (ArgumentNullException ex)
        {
            await WriteResponse(context, 400, new ErrorResponse
            {
                Type = "BadRequest",
                Message = ex.Message,
                StatusCode = 400
            });
        }
        catch (InvalidOperationException ex)
        {
            await WriteResponse(context, 422, new ErrorResponse
            {
                Type = "UnprocessableEntity",
                Message = ex.Message,
                StatusCode = 422
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponse(context, 401, new ErrorResponse
            {
                Type = "Unauthorized",
                Message = ex.Message,
                StatusCode = 401
            });
        }
        catch (KeyNotFoundException ex)
        {
            await WriteResponse(context, 404, new ErrorResponse
            {
                Type = "NotFound",
                Message = ex.Message,
                StatusCode = 404
            });
        }
        catch (Exception ex)
        {
            await WriteResponse(context, 500, new ErrorResponse
            {
                Type = "InternalServerError",
                Message = ex.Message,
                StatusCode = 500
            });
        }
    }

    private static async Task WriteResponse(HttpContext context, int statusCode, ErrorResponse error)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(error, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
