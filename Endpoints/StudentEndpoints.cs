using estudiantes_icinf.Common;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using FluentValidation;

namespace estudiantes_icinf.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students");

        group.MapGet("/", async (IStudentRepository repo) =>
        {
            var students = await repo.GetAllAsync();
            var response = ApiResponse.Ok(
                students,
                message: "Estudiantes obtenidos correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });

        group.MapGet("/{id:guid}", async (Guid id, IStudentRepository repo) =>
        {
            var student = await repo.GetByIdAsync(id);
            if (student is null)
            {
                var notFound = ApiResponse.Error(
                    $"No existe un estudiante con id '{id}'.",
                    StatusCodes.Status404NotFound);
                return Results.Json(notFound, statusCode: notFound.StatusCode);
            }

            var response = ApiResponse.Ok(
                student,
                message: "Estudiante obtenido correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });

        group.MapPost("/", async (
            CreateStudentDto dto,
            IValidator<CreateStudentDto> validator,
            IStudentRepository repo,
            HttpContext httpContext) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray();
                var invalid = ApiResponse.Error(
                    "Los datos enviados no son válidos.",
                    StatusCodes.Status400BadRequest,
                    errors);
                return Results.Json(invalid, statusCode: invalid.StatusCode);
            }

            if (await repo.GetByEmailAsync(dto.Email) is not null)
            {
                var conflict = ApiResponse.Error(
                    $"El correo '{dto.Email}' ya está registrado.",
                    StatusCodes.Status409Conflict);
                return Results.Json(conflict, statusCode: conflict.StatusCode);
            }

            var created = await repo.AddAsync(dto);
            httpContext.Response.Headers.Location = $"/api/students/{created.Id}";

            var response = ApiResponse.Ok(
                created,
                StatusCodes.Status201Created,
                "Estudiante creado correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });

        group.MapPatch("/{id:guid}", async (
            Guid id,
            UpdateStudentDto dto,
            IValidator<UpdateStudentDto> validator,
            IStudentRepository repo) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToArray();
                var invalid = ApiResponse.Error(
                    "Los datos enviados no son válidos.",
                    StatusCodes.Status400BadRequest,
                    errors);
                return Results.Json(invalid, statusCode: invalid.StatusCode);
            }

            if (dto.Email is not null && await repo.GetByEmailAsync(dto.Email, id) is not null)
            {
                var conflict = ApiResponse.Error(
                    $"El correo '{dto.Email}' ya está registrado por otro estudiante.",
                    StatusCodes.Status409Conflict);
                return Results.Json(conflict, statusCode: conflict.StatusCode);
            }

            var updated = await repo.UpdateAsync(id, dto);
            if (updated is null)
            {
                var notFound = ApiResponse.Error(
                    $"No existe un estudiante con id '{id}'.",
                    StatusCodes.Status404NotFound);
                return Results.Json(notFound, statusCode: notFound.StatusCode);
            }

            var response = ApiResponse.Ok(
                updated,
                message: "Estudiante actualizado correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IStudentRepository repo) =>
        {
            var student = await repo.GetByIdAsync(id);
            if (student is null)
            {
                var notFound = ApiResponse.Error(
                    $"No existe un estudiante con id '{id}'.",
                    StatusCodes.Status404NotFound);
                return Results.Json(notFound, statusCode: notFound.StatusCode);
            }

            await repo.DeleteAsync(id);

            var response = ApiResponse.Ok<object?>(
                null,
                message: "Estudiante eliminado correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });
    }
}
