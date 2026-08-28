using estudiantes_icinf.Common;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using FluentValidation;

// =========================================================================
// Fragmento de Endpoints/StudentEndpoints.cs a cargo de Gabriel Rivas
// Parte 3: GET /api/students y GET /api/students/{id}
// Parte 4: POST /api/students y PATCH /api/students/{id}
// =========================================================================

        // -----------------------------------------------------------------
        // Responsable: Estudiante 3 (reemplazar por nombre real)
        // Endpoints: GET /api/students  y  GET /api/students/{id}
        // -----------------------------------------------------------------
        group.MapGet("/", async (IStudentRepository repo) =>
        {
            var students = await repo.GetAllAsync();
            var response = ApiResponse.Ok(students, message: "Estudiantes obtenidos correctamente.");
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

            var response = ApiResponse.Ok(student, message: "Estudiante obtenido correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });

        // -----------------------------------------------------------------
        // Responsable: Estudiante 4 (reemplazar por nombre real)
        // Endpoints: POST /api/students  y  PATCH /api/students/{id}
        // -----------------------------------------------------------------
        group.MapPost("/", async (
            CreateStudentDto dto,
            IValidator<CreateStudentDto> validator,
            IStudentRepository repo) =>
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage);
                var invalid = ApiResponse.Error(
                    "Los datos enviados no son válidos.",
                    StatusCodes.Status400BadRequest,
                    errors);
                return Results.Json(invalid, statusCode: invalid.StatusCode);
            }

            if (await repo.EmailExistsAsync(dto.Email))
            {
                var conflict = ApiResponse.Error(
                    $"El correo '{dto.Email}' ya está registrado.",
                    StatusCodes.Status409Conflict);
                return Results.Json(conflict, statusCode: conflict.StatusCode);
            }

            var student = await repo.CreateAsync(new Student
            {
                Name = dto.Name,
                Email = dto.Email,
                Age = dto.Age
            });

            var response = ApiResponse.Ok(student, StatusCodes.Status201Created, "Estudiante creado correctamente.");
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
                var errors = validation.Errors.Select(e => e.ErrorMessage);
                var invalid = ApiResponse.Error(
                    "Los datos enviados no son válidos.",
                    StatusCodes.Status400BadRequest,
                    errors);
                return Results.Json(invalid, statusCode: invalid.StatusCode);
            }

            if (dto.Email is not null && await repo.EmailExistsAsync(dto.Email, excludeId: id))
            {
                var conflict = ApiResponse.Error(
                    $"El correo '{dto.Email}' ya está registrado por otro estudiante.",
                    StatusCodes.Status409Conflict);
                return Results.Json(conflict, statusCode: conflict.StatusCode);
            }

            var updated = await repo.UpdateAsync(id, student =>
            {
                if (dto.Name is not null) student.Name = dto.Name;
                if (dto.Email is not null) student.Email = dto.Email;
                if (dto.Age is not null) student.Age = dto.Age.Value;
            });

            if (updated is null)
            {
                var notFound = ApiResponse.Error(
                    $"No existe un estudiante con id '{id}'.",
                    StatusCodes.Status404NotFound);
                return Results.Json(notFound, statusCode: notFound.StatusCode);
            }

            var response = ApiResponse.Ok(updated, message: "Estudiante actualizado correctamente.");
            return Results.Json(response, statusCode: response.StatusCode);
        });
