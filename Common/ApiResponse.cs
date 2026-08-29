namespace estudiantes_icinf.Common;

// =========================================================================
// TALLER EVALUADO I - Estándar de respuestas HTTP JSON
// Parte 1: Diseño del estándar.
// Responsable: Roberto González
// =========================================================================
//
// Contrato único para TODAS las respuestas de la API (éxitos y errores):
//
// {
//   "success": true | false,          -> boolean, indica si la operación fue exitosa
//   "statusCode": 200,                 -> number, código HTTP real de la respuesta
//   "message": "string",               -> string, mensaje humano/legible del resultado
//   "dataType": "object|array|null",   -> string, tipo de dato JS/JSON contenido en "data"
//   "data": <T> | [<T>] | null,        -> generic, el recurso o lista de recursos (null en error)
//   "errors": ["string"] | null,       -> array de string, detalle de errores (null en éxito)
//   "timestamp": "2026-08-28T12:00:00Z"-> string ISO 8601, momento en que se generó la respuesta
// }
//
// - "data" es genérico (ApiResponse<T>) para poder representar tanto un objeto único
//   (GET /api/students/{id}) como una lista (GET /api/students) sin cambiar la forma
//   del contrato.
// - "errors" siempre es un arreglo de strings, incluso cuando hay un solo error, para
//   que el cliente no tenga que distinguir entre "un error" y "varios errores".
// - "dataType" existe para que un cliente débilmente tipado (JS) sepa, sin inspeccionar
//   "data", si debe esperar un objeto, un arreglo o nada.

/// <summary>
/// Estándar único de respuesta JSON de la API (éxito y error).
/// </summary>
/// <typeparam name="T">Tipo de dato contenido en <see cref="Data"/>.</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public string DataType { get; init; } = "null";
    public T? Data { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, int statusCode = 200, string message = "OK")
    {
        var dataType = data switch
        {
            null => "null",
            System.Collections.IEnumerable and not string => "array",
            _ => "object"
        };

        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = statusCode,
            Message = message,
            DataType = dataType,
            Data = data,
            Errors = null
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, int statusCode, IEnumerable<string>? errors = null)
    {
        var errorList = errors?.ToList();
        if (errorList is null || errorList.Count == 0)
        {
            errorList = [message];
        }

        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            DataType = "null",
            Data = default,
            Errors = errorList
        };
    }
}

/// <summary>
/// Helper no genérico para construir respuestas de error rápidamente
/// (por ejemplo desde el manejador global de excepciones), sin tener que
/// indicar un tipo T que no aplica.
/// </summary>
public static class ApiResponse
{
    public static ApiResponse<object?> Error(string message, int statusCode, IEnumerable<string>? errors = null)
        => ApiResponse<object?>.ErrorResponse(message, statusCode, errors);

    public static ApiResponse<T> Ok<T>(T data, int statusCode = 200, string message = "OK")
        => ApiResponse<T>.SuccessResponse(data, statusCode, message);
}
