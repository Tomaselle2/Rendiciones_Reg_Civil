using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RendicionesRegistroCivil.Utils.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context, Exception exception, CancellationToken ct)
        {
            var (status, title) = exception switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "Conflicto"),
                _ => (0, (string?)null)
            };

            // No es una excepción de negocio: dejar que responda un 500 genérico
            if (status == 0)
            {
                _logger.LogError(exception, "Error no controlado");
                return false;
            }

            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception.Message
            }, ct);

            return true;
        }
    }
}
