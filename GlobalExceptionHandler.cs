using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace estudiantes_icinf.Common;

// =========================================================================
// TALLER EVALUADO I - Estándar de respuestas HTTP JSON
// Parte 2: Implementación - Manejador global de errores.
// Responsable: Estudiante 2 (reemplazar por nombre real antes de hacer commit)
// =========================================================================
//
// Cualquier excepción NO controlada explícitamente por un endpoint cae aquí
// y se transforma en un ApiResponse<object?> con statusCode 500, en vez de
// devolver la página de error nativa de ASP.NET Core.
//
// Se registra en Program.cs con:
//   builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
//   builder.Services.AddProblemDetails();
//   app.UseExceptionHandler();

/// <summary>
/// Manejador global de excepciones no controladas. Estandariza cualquier
/// error inesperado al mismo contrato ApiResponse usado en el resto de la API.
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);

        var response = ApiResponse.Error(
            message: "Ocurrió un error inesperado al procesar la solicitud.",
            statusCode: StatusCodes.Status500InternalServerError,
            errors: new[] { exception.Message }
        );

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
