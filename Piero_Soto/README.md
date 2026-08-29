# ICINF1108-templates2026 · Taller Evaluado I

API demostrativa con .NET (ASP.NET Core Minimal API) para practicar consultas con Postman u otro cliente HTTP.

Rama de trabajo: `estudiantes_icinf-dotnet`.

## Requisitos

- .NET SDK 10

## Clonar el repositorio

```bash
git clone https://github.com/INF-UCT/ICINF1108-templates2026
cd ICINF1108-templates2026
git checkout estudiantes_icinf-dotnet
```

## Restaurar dependencias

```bash
dotnet restore
```

## Ejecutar el servidor

```bash
dotnet run
```

El servidor queda disponible en la URL que se muestre en consola (por ejemplo `http://localhost:5101`).

## Endpoints disponibles

La API expone operaciones CRUD completas sobre estudiantes bajo `/api/students`:

| Metodo | Ruta                    | Descripcion              |
|--------|-------------------------|---------------------------|
| POST   | `/api/students`         | Crear un estudiante       |
| GET    | `/api/students`         | Listar todos los estudiantes |
| GET    | `/api/students/:id`     | Buscar un estudiante por id |
| PATCH  | `/api/students/:id`     | Actualizar un estudiante  |
| DELETE | `/api/students/:id`     | Eliminar un estudiante    |

> El taller **no agrega endpoints nuevos**; solo estandariza la forma de sus respuestas (ver sección siguiente).

## Modelo de datos

Cada estudiante tiene:

| Campo       | Tipo             | Descripcion                          |
|-------------|------------------|----------------------------------------|
| `id`        | UUID             | Generado automaticamente al crear     |
| `name`      | string           | 3 a 100 caracteres                     |
| `email`     | string           | Direccion de correo valida y **unica** |
| `age`       | int              | Entre 18 y 99                          |
| `createdAt` | datetime (ISO)   | Generado automaticamente al crear      |
| `updatedAt` | datetime (ISO)   | Actualizado en cada `PATCH`            |

Ejemplo de body para `POST`:

```json
{
  "name": "Pedro Diaz",
  "email": "pedro.diaz@alu.uct.cl",
  "age": 22
}
```

`PATCH` acepta cualquier subconjunto de `name`, `email`, `age` (actualizacion parcial).

Los datos se guardan en `Data/students.json`.

## Documentacion interactiva

Swagger UI disponible en `http://localhost:5101/docs`

También puedes probar los endpoints directamente desde el archivo `estudiantes_icinf.http`.

---

## Estándar de respuestas HTTP JSON (Taller Evaluado I)

### Parte 1 — Diseño

Todas las respuestas de la API, sean éxito o error, comparten un único contrato JSON,
implementado en `Common/ApiResponse.cs` como una clase genérica `ApiResponse<T>`:

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Estudiantes obtenidos correctamente.",
  "dataType": "array",
  "data": [
    {
      "id": "5f8a1b2c-3d4e-4f5a-8b6c-7d8e9f0a1b2c",
      "name": "Pedro Diaz",
      "email": "pedro.diaz@alu.uct.cl",
      "age": 22,
      "createdAt": "2026-08-28T15:00:00Z",
      "updatedAt": "2026-08-28T15:00:00Z"
    }
  ],
  "errors": null,
  "timestamp": "2026-08-28T15:03:12Z"
}
```

Ejemplo de error (`409 Conflict` por email duplicado):

```json
{
  "success": false,
  "statusCode": 409,
  "message": "El correo 'pedro.diaz@alu.uct.cl' ya está registrado.",
  "dataType": "null",
  "data": null,
  "errors": ["El correo 'pedro.diaz@alu.uct.cl' ya está registrado."],
  "timestamp": "2026-08-28T15:05:40Z"
}
```

| Campo        | Tipo                    | Descripción                                                        |
|--------------|-------------------------|---------------------------------------------------------------------|
| `success`    | boolean                 | `true` si la operación fue exitosa, `false` en caso de error.      |
| `statusCode` | number                  | Código HTTP real de la respuesta (200, 201, 400, 404, 409, 500...). |
| `message`    | string                  | Mensaje legible para el cliente/consumidor de la API.               |
| `dataType`   | string                  | `"object"`, `"array"` o `"null"`; tipo de dato JS/JSON de `data`.    |
| `data`       | generic (`T`) o `null`  | El recurso, la lista de recursos, o `null` cuando hay error.        |
| `errors`     | array de string o `null`| Detalle de errores; `null` en respuestas exitosas.                  |
| `timestamp`  | string (ISO 8601)       | Momento en que se generó la respuesta.                              |

`data` es genérico (`ApiResponse<T>`) precisamente para poder representar un único objeto
(`GET /api/students/{id}`) o un arreglo (`GET /api/students`) sin cambiar la forma del contrato;
`dataType` le indica al cliente cuál de los dos casos aplica sin tener que inspeccionar `data`.

### Parte 2 — Implementación

- `Common/ApiResponse.cs`: define `ApiResponse<T>` y los métodos de fábrica `SuccessResponse` /
  `ErrorResponse`, además del helper estático `ApiResponse.Ok(...)` / `ApiResponse.Error(...)`
  usado desde los endpoints.
- `Common/GlobalExceptionHandler.cs`: implementa `IExceptionHandler` (registrado en `Program.cs`
  con `AddExceptionHandler<GlobalExceptionHandler>()` + `app.UseExceptionHandler()`) para que
  cualquier excepción no controlada también responda en el mismo formato, con `statusCode 500`.
- `Endpoints/StudentEndpoints.cs`: cada endpoint (`GET`, `GET/{id}`, `POST`, `PATCH`, `DELETE`)
  fue migrado para devolver `ApiResponse<T>` en vez del objeto crudo, incluyendo los casos
  `404 Not Found`, `400 Bad Request` (validación FluentValidation) y `409 Conflict` (email duplicado).

### Responsables por parte del taller

| Parte del taller                                              | Responsable      |
|-----------------------------------------------------------------|------------------|
| Parte 1 — Diseño del estándar (`ApiResponse<T>`)                | Estudiante 1 — *(nombre)* |
| Parte 2 — Manejador global de excepciones (`GlobalExceptionHandler`) | Estudiante 2 — *(nombre)* |
| Parte 2 — Estandarización de `GET /api/students` y `GET /api/students/{id}` | Estudiante 3 — *(nombre)* |
| Parte 2 — Estandarización de `POST /api/students` y `PATCH /api/students/{id}` | Estudiante 4 — *(nombre)* |
| Parte 2 — Estandarización de `DELETE /api/students/{id}` + revisión final y documentación | Estudiante 5 — *(nombre)* |

> Reemplazar cada `*(nombre)*` por el nombre/usuario real del integrante antes de hacer commit,
> y confirmar que cada uno abre su Pull Request desde la rama con su propio nombre de usuario
> (ver sección "Flujo de trabajo con Git" más abajo).

## Flujo de trabajo con Git (entrega del taller)

```bash
# 1. Desvincularse del template (borra el historial, no los archivos)
rm -rf .git                      # linux / git bash
rm -Recurse -Force .\.git        # windows - powershell

# 2. Historial nuevo y primer commit
git init -b main
git add .
git commit -m "Base: API sin estandarizar"

# 3. En GitHub crear un repo VACÍO (sin README ni .gitignore) y conectarlo
git remote add origin https://github.com/<usuario>/<repo>.git
git push -u origin main

# 4. Cada integrante crea SU rama desde main
git checkout -b <nombre-de-usuario>   # ej: git checkout -b jperez

# 5. Cada integrante trabaja SOLO su parte (ver tabla de responsables arriba),
#    hace commit y sube su rama:
git add .
git commit -m "Estandariza <lo que corresponda>"
git push -u origin <nombre-de-usuario>

# 6. Abrir un Pull Request de <nombre-de-usuario> hacia main en GitHub.
#    El líder del grupo revisa y hace el merge. Nadie pushea directo a main.
```

Requisito de entrega: **al menos 1 Pull Request por integrante fusionado a `main`**.
