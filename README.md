# Taller Evaluado I — API de estudiantes

API CRUD desarrollada con ASP.NET Core Minimal API para el Taller Evaluado I de ICINF1108.
Todas las respuestas HTTP JSON, tanto exitosas como de error, utilizan un contrato único.

## Integrantes y responsabilidades

| Integrante | Responsabilidad |
|---|---|
| Roberto González | Diseño de `ApiResponse<T>` y manejador global de excepciones |
| Gabriel Rivas | Estandarización de GET, POST y PATCH |
| Piero Soto | Estandarización de DELETE, revisión final y documentación |

Los documentos individuales se conservan en [`Contribuciones`](./Contribuciones).

## Requisitos

- .NET SDK 10
- Git

## Instalación y ejecución

```bash
git clone https://github.com/rgonzalez2026-ui/Taller_1.git
cd Taller_1
dotnet restore
dotnet run
```

Con el perfil HTTP incluido, la API queda disponible en `http://localhost:5101` y Swagger UI en
`http://localhost:5101/docs`.

## Endpoints

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/students` | Lista todos los estudiantes |
| GET | `/api/students/{id}` | Obtiene un estudiante por UUID |
| POST | `/api/students` | Crea un estudiante |
| PATCH | `/api/students/{id}` | Actualiza parcialmente un estudiante |
| DELETE | `/api/students/{id}` | Elimina un estudiante |

El taller no agrega endpoints nuevos; solamente estandariza sus respuestas y el manejo de errores.

## Estándar de respuestas HTTP JSON

El contrato compartido se implementa en `Common/ApiResponse.cs` y contiene siete campos:

| Campo | Tipo | Descripción |
|---|---|---|
| `success` | boolean | Indica si la operación fue exitosa |
| `statusCode` | number | Código HTTP real de la respuesta |
| `message` | string | Descripción legible del resultado |
| `dataType` | string | `object`, `array` o `null` |
| `data` | genérico o null | Objeto, lista o ausencia de datos |
| `errors` | array de string o null | Detalles de validación o del error |
| `timestamp` | string ISO 8601 | Momento UTC de generación de la respuesta |

### Ejemplo exitoso con una lista

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Estudiantes obtenidos correctamente.",
  "dataType": "array",
  "data": [
    {
      "id": "5f8a1b2c-3d4e-4f5a-8b6c-7d8e9f0a1b2c",
      "name": "Juan Perez",
      "email": "juan.perez@alu.uct.cl",
      "age": 20,
      "createdAt": "2026-08-24T12:00:00Z",
      "updatedAt": "2026-08-24T12:00:00Z"
    }
  ],
  "errors": null,
  "timestamp": "2026-08-28T20:00:00Z"
}
```

### Ejemplo exitoso con un objeto

```json
{
  "success": true,
  "statusCode": 200,
  "message": "Estudiante obtenido correctamente.",
  "dataType": "object",
  "data": {
    "id": "5f8a1b2c-3d4e-4f5a-8b6c-7d8e9f0a1b2c",
    "name": "Juan Perez",
    "email": "juan.perez@alu.uct.cl",
    "age": 20,
    "createdAt": "2026-08-24T12:00:00Z",
    "updatedAt": "2026-08-24T12:00:00Z"
  },
  "errors": null,
  "timestamp": "2026-08-28T20:00:00Z"
}
```

### Ejemplo de error

```json
{
  "success": false,
  "statusCode": 404,
  "message": "No existe un estudiante con id '00000000-0000-0000-0000-000000000000'.",
  "dataType": "null",
  "data": null,
  "errors": [
    "No existe un estudiante con id '00000000-0000-0000-0000-000000000000'."
  ],
  "timestamp": "2026-08-28T20:00:00Z"
}
```

## Validaciones y errores cubiertos

- Datos inválidos en POST y PATCH: `400 Bad Request`.
- JSON malformado: `400 Bad Request`.
- Estudiante o ruta inexistente: `404 Not Found`.
- Correo duplicado: `409 Conflict`.
- Método HTTP no permitido: `405 Method Not Allowed`.
- Tipo de contenido incorrecto: `415 Unsupported Media Type`.
- Excepciones no controladas: `500 Internal Server Error`.

Los errores de endpoint se construyen con `ApiResponse.Error(...)`. Las excepciones inesperadas se
procesan mediante `GlobalExceptionHandler`, registrado globalmente en `Program.cs`. Las respuestas
vacías producidas por el framework se completan con el mismo contrato mediante `UseStatusCodePages`.

## Pruebas manuales

El archivo `estudiantes_icinf.http` contiene solicitudes listas para ejecutar. También se puede usar
Swagger UI o Postman. Los datos se guardan localmente en `Data/students.json`.

## Flujo Git de entrega

Cada integrante trabaja desde una rama propia y abre un Pull Request hacia `main`:

- `RobertoG`
- `GabrielR`
- `PieroSotoUCT`

No se deben hacer pushes directos a `main`. La rama `main` debe contener siempre la versión integrada
y ejecutable después de revisar y fusionar los Pull Requests.
