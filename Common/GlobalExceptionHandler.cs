using Microsoft.AspNetCore.Diagnostics;

namespace estudiantes_icinf.Common;

// =========================================================================
// TALLER EVALUADO I - Estándar de respuestas HTTP JSON
// Parte 2: Implementación - Manejador global de errores.
// Responsable: Roberto González
// =========================================================================
//
// Cualquier excepción NO controlada explícitamente por un endpoint cae aquí
// y se transforma en un ApiResponse<object?>. Las solicitudes JSON inválidas
// conservan su statusCode 400 y los errores inesperados responden con 500.
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
        var statusCode = exception is BadHttpRequestException badRequest
            ? badRequest.StatusCode
            : StatusCodes.Status500InternalServerError;

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Excepción no controlada: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "Solicitud HTTP inválida: {Message}", exception.Message);
        }

        var message = statusCode == StatusCodes.Status400BadRequest
            ? "La solicitud contiene datos JSON inválidos."
            : "Ocurrió un error inesperado al procesar la solicitud.";

        var response = ApiResponse.Error(
            message: message,
            statusCode: statusCode
        );

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }
}
