using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Application.Exceptions;
using SmartWarehouse.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartWarehouse.WebAPI.Middlewares
{
    /// <summary>
    /// Middleware xử lý ngoại lệ toàn cục chuẩn RFC 7807 ProblemDetails và đính kèm X-Correlation-ID.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            string correlationId = context.Items.TryGetValue(RequestCorrelationMiddleware.CorrelationIdHeader, out var cId)
                ? cId?.ToString() ?? Guid.NewGuid().ToString()
                : Guid.NewGuid().ToString();

            int statusCode = exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                DuplicateWarehouseCodeException => StatusCodes.Status409Conflict,
                DuplicateSkuException => StatusCodes.Status409Conflict,
                InsufficientStockException => StatusCodes.Status409Conflict,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            _logger.LogError(exception, "[CorrelationID: {CorrelationId}] Handled exception: {Message}",
                correlationId, exception.Message);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode switch
                {
                    StatusCodes.Status400BadRequest => "Bad Request",
                    StatusCodes.Status404NotFound => "Not Found",
                    StatusCodes.Status409Conflict => "Conflict",
                    _ => "Internal Server Error"
                },
                Detail = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred. Please try again later."
                    : exception.Message,
                Instance = context.Request.Path
            };

            problem.Extensions["correlationId"] = correlationId;

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem, jsonOptions));
        }
    }
}
