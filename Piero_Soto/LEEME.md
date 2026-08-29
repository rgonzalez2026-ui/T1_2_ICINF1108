# Piero Soto — Parte 5

## Parte 5 — Estandarización de DELETE + revisión final y documentación
**Endpoint:** `DELETE /api/students/{id}`
Responde con `ApiResponse.Ok<object?>(null, ...)` en caso de éxito y `ApiResponse.Error(...)` con
`404 Not Found` si el estudiante no existe.

Además, esta parte incluye la **revisión final de consistencia** del estándar en todos los
endpoints y la **documentación** del estándar en el `README.md` (sección "Estándar de respuestas
HTTP JSON", con el diseño del contrato, ejemplos de éxito/error y la tabla de responsables).

---
*Extraído de `Endpoints/StudentEndpoints.cs` (bloque DELETE, en `StudentEndpoints_Parte5.cs`) y
del `README.md` completo de la rama `estudiantes_icinf-dotnet`.*
