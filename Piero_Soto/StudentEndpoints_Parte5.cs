using estudiantes_icinf.Common;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;

// =========================================================================
// Fragmento de Endpoints/StudentEndpoints.cs a cargo de Piero Soto
// Parte 5: DELETE /api/students/{id} + revisión final del estándar
// =========================================================================

        // -----------------------------------------------------------------
        // Responsable: Estudiante 5 (reemplazar por nombre real)
        // Endpoint: DELETE /api/students/{id}
        // + revisión final de consistencia del estándar en todos los
        //   endpoints y actualización de la sección de README/Swagger.
        // -----------------------------------------------------------------
        group.MapDelete("/{id:guid}", async (Guid id, IStudentRepository repo) =>
        {
            var deleted = await repo.DeleteAsync(id);
            if (!deleted)
            {
                var notFound = ApiResponse.Error(
                    $"No existe un estudiante con id '{id}'.",
                    StatusCodes.Status404NotFound);
                return Results.Json(notFound, statusCode: notFound.StatusCode);
            }

            var response = ApiResponse.Ok<object?>(null, message: "Estudiante eliminado correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });
