# Gabriel Rivas — Partes 3 y 4

## Parte 3 — Estandarización de GET
**Endpoints:** `GET /api/students` y `GET /api/students/{id}`
Ambos devuelven ahora un `ApiResponse<T>` mediante `ApiResponse.Ok(...)`. El caso de estudiante
no encontrado responde con `ApiResponse.Error(...)` y `404 Not Found`.

## Parte 4 — Estandarización de POST y PATCH
**Endpoints:** `POST /api/students` y `PATCH /api/students/{id}`
Cubre los casos: validación fallida (`400 Bad Request` vía FluentValidation), correo duplicado
(`409 Conflict`), estudiante no encontrado en el PATCH (`404 Not Found`) y creación/actualización
exitosa (`201 Created` / `200 OK`), todos usando `ApiResponse<T>`.

---
*Aporte integrado y adaptado a la interfaz real del proyecto en `Endpoints/StudentEndpoints.cs`.*
