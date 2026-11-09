# Video Tutorial · API en C# con Docker y un agente de IA

[Ver el Video Tutorial en YouTube](https://www.youtube.com/watch?v=-iw6imTxQl4)

Material de Prof. Juan Zamora: CRUD de productos con ASP.NET Core 10, controllers, servicio, Entity Framework Core 10 y PostgreSQL. Swagger UI permite explorar y probar el contrato HTTP. No incluye una aplicación gráfica para usuarios finales.

## Requisitos antes de empezar

Conocimientos: clases y métodos de C#, `async/await` básico, JSON, métodos HTTP y nociones de tabla/clave primaria. Se explican durante el tutorial las piezas nuevas del framework.

Herramientas de la ruta principal:

- Docker Desktop con **motor activo** y Compose v2. Tener el icono abierto o el cliente instalado no garantiza que el servidor responda.
- .NET SDK 10 para crear, compilar y generar migraciones desde la terminal del equipo. Para levantar la referencia terminada únicamente con Compose, el SDK está en la imagen de construcción y no es necesario en el host.
- Git y un editor. El soporte de C# del editor ayuda a navegar, pero no sustituye al SDK.
- Un agente de código ya configurado, como Codex o Claude Code. Los comandos del proyecto se revisan independientemente del agente.
- Python 3, opcional, para ejecutar la comprobación HTTP del material; también se puede revisar desde Swagger UI o curl.

Si obtuviste el material como ZIP, ejecuta primero `git init` en su carpeta y guarda un commit inicial de la referencia. Si lo clonaste, ya tendrá su historial.

Comprueba **desde la carpeta del proyecto**, sin mostrar credenciales:

```sh
pwd
git status --short
dotnet --list-sdks
docker version
docker compose version
```

`docker version` debe mostrar el servidor además del cliente. El proyecto usa `global.json` para seleccionar .NET 10 y tiene versiones NuGet exactas más `packages.lock.json`.

## Levantar la referencia con Docker

```sh
cp .env.example .env
# Edita la contraseña del laboratorio en .env.
docker compose up --build -d
```

En PowerShell, la copia equivalente es `Copy-Item .env.example .env`.

1. `db` inicia PostgreSQL 17 con un volumen propio y espera a que acepte conexiones.
2. `migrate` aplica la migración ya revisada y termina.
3. `api` arranca cuando esa migración terminó correctamente.

Abre http://localhost:5210/swagger/index.html. El documento OpenAPI está en http://localhost:5210/openapi/v1.json. PostgreSQL no publica un puerto al host en esta configuración; la API lo alcanza por el nombre `db` en la red de Compose. El puerto de la API se limita a la interfaz local del equipo.

Swagger/OpenAPI están habilitados en Development para este laboratorio. La API no incluye autenticación ni una interfaz de negocio para clientes finales.

## Probar un CRUD real

```sh
curl -i http://localhost:5210/api/productos
curl -i -H 'Content-Type: application/json' \
  -d '{"nombre":"Cuaderno","precio":4.50,"existencias":8}' \
  http://localhost:5210/api/productos
```

Usa el `id` devuelto por POST para los siguientes ejemplos; aquí se muestra `1` únicamente como ejemplo:

```sh
curl -i http://localhost:5210/api/productos/1
curl -i -X PUT -H 'Content-Type: application/json' \
  -d '{"nombre":"Cuaderno grande","precio":6.25,"existencias":4}' \
  http://localhost:5210/api/productos/1
curl -i -X DELETE http://localhost:5210/api/productos/1
```

| Operación | Resultado esperado |
| --- | --- |
| GET colección / por ID | 200 y JSON |
| POST válido | 201, cuerpo y cabecera Location |
| PUT válido | 204 sin cuerpo |
| DELETE válido | 204 sin cuerpo |
| Nombre vacío, precio/existencias negativos, campo requerido ausente | 400 |
| Precio con más de dos decimales | 400 |
| Recurso inexistente | 404 |

No se implementa PATCH en este primer CRUD. PUT exige todos los campos editables. `page` empieza en 1 y `pageSize` admite 1–100.

Comprobación reproducible, que crea y elimina sus propios productos:

```sh
python3 scripts/verificar_http.py http://localhost:5210
```

## Qué archivo explica cada parte

- `Controllers/ProductosController.cs`: rutas, binding de parámetros y respuestas HTTP.
- `Contracts/ProductoDtos.cs`: contrato de entrada/salida y validación; el cliente no elige el Id.
- `Services/ProductosService.cs`: coordinación de las operaciones; normaliza el nombre y llama al contexto de datos. Un service puede existir por reglas de negocio aunque no haya BD; aquí se separa para recorrer y revisar cada responsabilidad.
- `Data/ProductosDbContext.cs`: mapeo de la entidad, precisión decimal y restricciones de tabla.
- `Models/Producto.cs`: entidad persistida.
- `Data/Migrations/`: cambios versionados del esquema.
- `Program.cs`: registro de dependencias, entorno, documentación y rutas.
- `Dockerfile` y `compose.yaml`: construcción, runtime, red, migraciones y volumen.

EF Core traduce las expresiones LINQ a SQL y guarda los cambios. Se muestran tanto las expresiones C# como las consultas registradas y el esquema SQL generado. No se construyen consultas concatenando texto del usuario.

## Ver la tabla y la persistencia

```sh
docker compose exec db psql -U profe -d productos \
  -c 'SELECT "Id", "Nombre", "Precio", "Existencias" FROM productos;'
docker compose restart db api
```

Después del reinicio, consulta de nuevo el producto. `restart` conserva el volumen. `docker compose down` detiene los servicios y conserva ese volumen; añadir `-v` lo eliminaría, así que esa opción no forma parte del procedimiento habitual.

## Entender y revisar las migraciones

```sh
dotnet tool restore
dotnet ef migrations script --project src/Productos.Api --output docs/migracion-revisada.sql
```

La generación del script necesita una cadena de conexión de laboratorio configurada para crear el contexto. El SQL inicial revisado ya está en `docs/migracion-inicial.sql` y se puede leer sin arrancar un motor.

La aplicación aplica migraciones únicamente cuando se ejecuta con `--migrate`; Compose lo hace en el servicio separado. No se mezcla `EnsureCreated` con las migraciones. Para cambiar el modelo, primero revisar el diff, después crear una nueva migración y leer su SQL antes de aplicarlo.

## Recorrido guiado y comprobaciones

Lee `docs/PROMPT-AGENTE.md` y `docs/REVISION-DEL-AUTOR.md`.

Estado al 8 de noviembre de 2026: **verificado en Docker Compose** con .NET / ASP.NET Core 10.0.14 y PostgreSQL 17.11. La migración termina con código 0 y la API arranca después. `evidencia/http.json` registra 18 peticiones con los estados esperados contra el contenedor; `persistencia-docker.json` demuestra que el producto permanece tanto al reiniciar como al recrear los contenedores de API y BD. Swagger UI se comprobó en el navegador y se capturó el SQL de las cuatro operaciones CRUD. El contenedor de la API usa únicamente el runtime y ejecuta con UID 1654.

Abre http://localhost:5210/swagger/index.html para la ruta Docker que seguirá el espectador.

La imagen se construye una sola vez desde el servicio `api`; `migrate` utiliza la misma imagen local y no intenta construirla o descargarla por separado. Esto evita un conflicto de publicación de la misma etiqueta durante builds paralelos.

Fuentes oficiales consultadas:

- https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
- https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0
- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-10.0
- https://www.npgsql.org/efcore/release-notes/10.0.html
- https://docs.docker.com/guides/dotnet/
