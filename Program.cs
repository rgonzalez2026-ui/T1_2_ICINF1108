using estudiantes_icinf.Common;
using estudiantes_icinf.Endpoints;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddSingleton<IStudentRepository, JsonStudentRepository>();
builder.Services.AddScoped<IValidator<CreateStudentDto>, CreateStudentValidator>();
builder.Services.AddScoped<IValidator<UpdateStudentDto>, UpdateStudentValidator>();

var app = builder.Build();

app.UseExceptionHandler();

app.UseStatusCodePages(async context =>
{
    var httpResponse = context.HttpContext.Response;
    if (httpResponse.HasStarted || httpResponse.ContentLength is > 0)
    {
        return;
    }

    var message = httpResponse.StatusCode switch
    {
        StatusCodes.Status400BadRequest => "La solicitud no es válida.",
        StatusCodes.Status401Unauthorized => "La solicitud requiere autenticación.",
        StatusCodes.Status403Forbidden => "No tienes permisos para realizar esta operación.",
        StatusCodes.Status404NotFound => "El recurso solicitado no existe.",
        StatusCodes.Status405MethodNotAllowed => "El método HTTP no está permitido para esta ruta.",
        StatusCodes.Status415UnsupportedMediaType => "El tipo de contenido enviado no es compatible.",
        _ => "La solicitud no pudo ser procesada."
    };

    var response = ApiResponse.Error(message, httpResponse.StatusCode);
    httpResponse.ContentType = "application/json; charset=utf-8";
    await httpResponse.WriteAsJsonAsync(response);
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "docs";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de estudiantes v1");
});

app.UseHttpsRedirection();

app.MapStudentEndpoints();

app.Run();
