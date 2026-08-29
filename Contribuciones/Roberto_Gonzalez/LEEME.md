# Roberto González — Partes 1 y 2

## Parte 1 — Diseño del estándar de respuesta
**Archivo:** `Common/ApiResponse.cs`
Define la clase genérica `ApiResponse<T>` (el contrato único para éxitos y errores) con los
métodos de fábrica `SuccessResponse` / `ErrorResponse` y el helper estático `ApiResponse.Ok(...)`
/ `ApiResponse.Error(...)` usado por todos los endpoints. `ErrorResponse` garantiza además que
`errors` contenga al menos el mensaje principal, incluso si recibe una colección vacía.

## Parte 2 — Manejador global de excepciones
**Archivo:** `Common/GlobalExceptionHandler.cs`
Implementa `IExceptionHandler` (registrado en `Program.cs`) para que cualquier excepción no
controlada por el código también responda usando el mismo formato `ApiResponse<T>`, con
`statusCode 500`.

---
*Aporte integrado en `Common/ApiResponse.cs`, `Common/GlobalExceptionHandler.cs` y `Program.cs`.*
