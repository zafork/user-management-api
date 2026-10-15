# User Management API

API REST en .NET 8 para gestionar usuarios, con autenticación JWT y autorización por permisos, construida con arquitectura hexagonal y Entity Framework Core.

## Base de datos

| Dato | Valor |
|---|---|
| Motor | Oracle Database Free **23.26.3** (Oracle AI Database 26ai) |
| Imagen Docker | `gvenzl/oracle-free:23.26.3-slim` |
| Host y puerto | `localhost:1521` |
| Servicio (PDB) | `FREEPDB1` |
| Usuario de la aplicación | `app` / `AppPass123` |
| Administrador de Oracle | `SYSTEM` / `OraclePass123` |

La cadena de conexión se lee de `ConnectionStrings:DefaultConnection`, definida para desarrollo en `src/UserManagement.Api/appsettings.Development.json`.

## Requisitos

- .NET SDK 8.0.131 o superior dentro de la rama 8.0 (lo fija `global.json`)
- Docker con Docker Compose

## Cómo ejecutar

```bash
docker compose up -d
docker compose ps
dotnet tool restore
dotnet ef database update -p src/UserManagement.Infrastructure -s src/UserManagement.Api
dotnet run --project src/UserManagement.Api
```

1. `docker compose up -d` levanta Oracle. El primer arranque tarda 1–2 minutos; espera a que `docker compose ps` muestre `healthy`.
2. `dotnet tool restore` instala `dotnet-ef` en la versión del repositorio.
3. `dotnet ef database update` aplica las migraciones: crea el esquema y carga los usuarios de prueba.
4. `dotnet run` levanta la API en `http://localhost:5074`. Swagger: `http://localhost:5074/swagger`.

Para usar otra instancia de Oracle 23, crea el usuario `app` con esos datos o cambia la cadena de conexión.

## Usuarios de prueba

| Email | Contraseña | Rol | Permisos | Estado |
|---|---|---|---|---|
| `admin@demo.com` | `Admin123!` | Administrador | `users.read`, `users.update` | Activo |
| `consulta@demo.com` | `Consulta123!` | Consulta | `users.read` | Activo |
| `inactivo@demo.com` | `Inactivo123!` | Consulta | `users.read` | Inactivo |

## Endpoints

| Método y ruta | Acceso | Descripción |
|---|---|---|
| `POST /api/v1/auth/token` | Público | Recibe email y contraseña. Devuelve `accessToken`, `tokenType` y `expiresIn` (segundos). |
| `GET /api/v1/users?page=1&pageSize=10` | JWT válido | Lista usuarios paginados (`pageSize` máximo 100), sin contraseña ni hash. |
| `PUT /api/v1/users/{id}` | JWT válido + `users.update` | Actualiza nombre, email y estado activo. |

`src/UserManagement.Api/UserManagement.Api.http` tiene una llamada a cada endpoint. Los errores responden con *Problem Details* (RFC 9457): 400, 401, 403, 404 y 409.

## Pruebas

```bash
dotnet test
```

Pruebas unitarias del dominio y de los casos de uso de token y de actualización de usuario, con dobles de los puertos (sin base de datos).

## Decisiones de diseño

**Patrones y por qué**

- **Arquitectura hexagonal en cuatro proyectos.** Domain y Application no referencian EF Core, ASP.NET Core ni librerías de JWT: el núcleo se prueba sin infraestructura y el compilador impide romper la regla de dependencias.
- **Casos de uso con puertos de entrada y de salida.** Cada operación es una clase detrás de una interfaz. Los controllers solo traducen HTTP y la composición vive en `Program.cs`.
- **Modelo de dominio rico.** `User` valida sus reglas (email válido y normalizado, nombre obligatorio) y solo cambia por métodos.
- **Repository + Unit of Work.** `IUserRepository` expone consultas del negocio; `AppDbContext` implementa `IUnitOfWork`.
- **Autorización por permiso.** El token lleva un claim `permission` por cada permiso de los roles del usuario, y hay una política de ASP.NET Core por permiso: `[Authorize(Policy = "users.update")]`. Ningún controller tiene `if` de permisos. `GET /users` exige solo un JWT válido, como indica el enunciado.
- **Errores centralizados.** Las excepciones de dominio y de aplicación se traducen a *Problem Details* en un único `IExceptionHandler`.
- **Datos semilla en la migración.** Permisos, roles y usuarios (con hashes BCrypt precalculados) se cargan con `HasData`, así el esquema y los datos de prueba quedan versionados juntos.
- **Email único con índice en la base.** El caso de uso responde 409 si el email pertenece a otro usuario, y el índice único cubre el caso de dos actualizaciones simultáneas: el error de Oracle (`ORA-00001`) se traduce también a 409.
- **Configuración.** Clave, emisor, audiencia y duración del JWT se leen de configuración. Los valores de `appsettings.Development.json` son solo de desarrollo; en otros entornos se reemplazan con variables de entorno (`ConnectionStrings__DefaultConnection`, `Jwt__Key`).

**Para una siguiente iteración**

- Refresh tokens y revocación de sesiones.
- Límite de intentos en `/auth/token` contra fuerza bruta.
- Pruebas de integración contra Oracle real con Testcontainers.
- Secretos en un gestor (Key Vault, Secrets Manager) en lugar de archivos de configuración.
- Concurrencia optimista en la actualización de usuarios.
